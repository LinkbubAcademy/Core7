﻿using Common.Lib.Authentication;
using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Services;
using Common.Lib.Infrastructure;

namespace Common.Lib.Core
{
    public partial class Entity
    {
        #region Persisted properties

        /// <summary>
        /// Entity unique Id
        /// </summary>
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        #endregion

        #region Not persisted properties

        /// <summary>
        /// The unmodified Entity, used to get changes after
        /// the entty is cloned and modified
        /// </summary>
        public Entity? Origin { get; set; }

        public bool IsNew { get; set; } = true;

        public IContextFactory? ContextFactory { get; set; }

        public Func<Task<SaveResult>>? SaveAction { get; set; }

        public bool IsAlreadyAssigned { get; set; }

        internal ValidationResult? ValidationResult { get; set; }

        internal List<Action>? NotificationActions { get; set; }

        #endregion

        public Entity()
        {

        }
        public Entity(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        #region Clone

        /// <summary>
        /// Base for calling Clone more simple
        /// </summary>
        /// <returns>the cloned entity</returns>
        public Entity Clone()
        {
            return Clone<Entity>();
        }

        /// <summary>
        /// Method used to reproduce the entity, including itself
        /// as Origin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Clone<T>() where T : Entity, new()
        {
            var output = new T
            {
                Origin = this,
                Id = Id
            };

            return output;
        }

        public virtual Task<Dictionary<Guid, Entity>> IncludeChildren(QueryResult qr, int nestingLevel)
        {
            return Task.FromResult(new Dictionary<Guid, Entity>());
        }

        public virtual void AssignChildren(QueryResult qr)
        {            
        }

        public void Do<T>(Action<T> action) where T : Entity, new()
        {
            action(this as T);
        }

        #endregion

        #region Changes

        /// <summary>
        /// Collect Entity Changes. This method requires to have used Clone tp modify the entiy
        /// when it is an update
        /// </summary>
        /// <returns></returns>
        public virtual EntityInfo GetChanges()
        {
            var currentTypeName = this.GetType().FullName;

            if (currentTypeName == null)
                throw new ArgumentException("Entity objects must have a reflection Type");

            var output = new EntityInfo
            {
                EntityId = Id,
                EntityModelType = currentTypeName,
                IsNew = IsNew
            };

            if (Origin == null)
            {
                this.CreatedOn = DateTime.Now;
                output.AddChange(EntityMetadata.CreatedOn, this.CreatedOn);
            }

            this.UpdatedOn = DateTime.Now;
            output.AddChange(EntityMetadata.UpdatedOn, this.UpdatedOn);

            return output;
        }


        /// <summary>
        /// This method applies the changes that come from services
        /// Todo: optimize foreachs to avoid looping n (changes) x m (inheritances steps)
        /// </summary>
        /// <param name="changes">the list of changes to apply</param>
        public virtual List<ChangeUnit> ApplyChanges(List<ChangeUnit> changes)
        {
            changes = changes.OrderBy(x => x.MetadataId).ToList();

            foreach (var change in changes)
            {
                switch (change.MetadataId)
                {
                    case EntityMetadata.Id:
                        Id = change.GetValueAsGuid();
                        break;
                    case EntityMetadata.CreatedOn:
                        CreatedOn = change.GetValueAsDateTime();
                        break;
                    case EntityMetadata.UpdatedOn:
                        UpdatedOn = change.GetValueAsDateTime();
                        break;
                    default:
                        return changes.Where(x => x.MetadataId > EntityMetadata.Last).ToList();
                }
            }

            return changes.Where(x => x.MetadataId > EntityMetadata.Last).ToList();
        }

        #endregion

        #region Save

        public virtual async Task<IEnumerable<string>?> ValidateNew()
        {
            return await Task.FromResult(default(IEnumerable<string>)); 
        }

        public virtual async Task<IEnumerable<string>?> ValidateUpdate()
        {
            return await Task.FromResult(default(IEnumerable<string>));
        }
        public virtual async Task<IEnumerable<string>?> ValidateDelete()
        {
            return await Task.FromResult(default(IEnumerable<string>));
        }


        public virtual void Save(IUnitOfWork uow)
        {
            uow.AddEntitySave(this);
        }

        public virtual async Task<SaveResult> SaveAsync<T>(IUnitOfWork? uow = null) where T : Entity, new()
        {
            if (this is not T entity)
                return await Task.FromResult(new SaveResult()
                {
                    IsSuccess = false,
                    Message = $"Entity of type {this.GetType().Name} cannot be saved as {typeof(T).Name}"
                });

            if (ContextFactory == null)
                throw new ContextFactoryNullException("Entity", "SaveAsync");

            if (uow != null && !ContextFactory.IsServerMode)
                throw new ApplicationException("You cannot use SaveAsync with UnitOfWork in client mode. Use Save instead");

            using var repo = ContextFactory.GetRepository<T>(uow) ?? 
                throw new RepositoryNotRegisteredException(typeof(T));

            var output = new SaveResult();

            if (IsNew || Id == default)
            {
                var createErrors = await ValidateNew();
                if (createErrors == null)
                {
                    var addResult = await repo.AddAsync(entity);
                    output.IsSuccess = addResult.IsSuccess;

                    if (!addResult.IsSuccess)
                        output.AddError(addResult.Message);
                }
                else
                {
                    output.IsSuccess = false;
                    createErrors.DoForeach(x => output.AddError(x));
                }
            }
            else
            {
                var updateErrors = await ValidateUpdate();
                if (updateErrors == null)
                {
                    var updateResult = await repo.UpdateAsync(entity);
                    output.IsSuccess = updateResult.IsSuccess;

                    if (!updateResult.IsSuccess)
                        output.AddError(updateResult.Message);
                }
                else
                {
                    output.IsSuccess = false;
                    updateErrors.DoForeach(x => output.AddError(x));
                }
            }

            return output;
        }

        public virtual async Task<DeleteResult> DeleteAsync<T>(IUnitOfWork? uow = null) where T : Entity, new()
        {           

            if (ContextFactory == null)
                throw new ContextFactoryNullException("Entity", "DeleteAsync");

            if (uow != null && !ContextFactory.IsServerMode)
                throw new ApplicationException("You cannot use DeleteAsync with UnitOfWork in client mode. Use Save instead");

            using var repo = ContextFactory.GetRepository<T>(uow) ??
                throw new RepositoryNotRegisteredException(typeof(T));

            var output = new DeleteResult();

            var deleteErrors = await ValidateDelete();
            if (deleteErrors == null)
            {
                var deleteResult = await repo.DeleteAsync(this.Id);
                output.IsSuccess = deleteResult.IsSuccess;

                if (!deleteResult.IsSuccess)
                    output.AddError(deleteResult.Message);
            }
            else
            {
                output.IsSuccess = false;
                deleteErrors.DoForeach(x => output.AddError(x));
            }
            

            return output;
        }

        #endregion

        #region Parametric actions

        public virtual async Task<IActionResult?> RequestParametricActionAsync(string repoType, string actionName, object[] actionParams)
        {
            if (ContextFactory == null)
                throw new InvalidOperationException("you must get a model through the propel channels, ContextFactory is missing");

            if (ContextFactory.IsServerMode)
                return null;

            var svcInvoker = ContextFactory.Resolve<IServiceInvoker>();

            var paramsCarrierFactory = ContextFactory.Resolve<IParamsCarrierFactory>();

            var userId = Guid.NewGuid(); //todo: implement user auth
            var userToken = string.Empty;//todo: implement user auth
            var paramsCarrier = paramsCarrierFactory.CreateParametricActionParams(
                                                            userId,
                                                            userToken,
                                                            DateTime.Now,
                                                            repoType,
                                                            this.Id,
                                                            actionName,
                                                            actionParams);

            var result = await svcInvoker.RequestParametricActionAsync(paramsCarrier);
            return result;
        }

        public virtual async Task<IActionResult?> ProcessActionAsync(string actionId, object[] actionParams)
        {
            return null; 
        }
        
        public T FindEntity<T>(Guid id) where T : Entity, new()
        {
            if (ContextFactory == null || !ContextFactory.IsServerMode)
                throw new InvalidOperationException("Wrong entity context");

            using var repo = ContextFactory.GetRepository<T>();
            var qr = repo.FindAsync(id).Result;

            return qr.IsSuccess ? qr.Value : null;
        }
        #endregion
    }

    public partial class Entity
    {
        #region metadata maps

        public static Dictionary<int, ValueTypes> MetadataMaps { get; set; } = new Dictionary<int, ValueTypes>()
        {
            { EntityMetadata.Id, ValueTypes.Guid },
            { EntityMetadata.CreatedOn, ValueTypes.DateTime },
            { EntityMetadata.UpdatedOn, ValueTypes.DateTime }
        };

        public static RolActionMap EntityRolesMap
        {
            get
            {
                return _entityRolesMap;
            }
        }
        static RolActionMap? _entityRolesMap;


        public virtual RolActionMap GetModelRolesMap()
        {
            return EntityRolesMap;
        }

        #endregion
    }

    public static class EntityMetadata
    {
        public const int Id = 1;
        public const int CreatedOn = 2;
        public const int UpdatedOn = 3;
        public const int Last = UpdatedOn;

        public static void RegisterParametricActions()
        {

        }

        public static Dictionary<string, Dictionary<int, Func<object?, string>>> ParamActionsParamsSerializer { get; set; } = new();

        public static Dictionary<string, Dictionary<int, Func<string, object?>>> ParamActionsParamsDeserializer { get; set; } = new();

        public static string[] SerializeParamActionValues(string paramActionId, object[] values)
        {
            var paramsConverter = ParamActionsParamsSerializer[paramActionId];

            var output = new string[values.Length];

            for (var i = 0; i < values.Length; i++)
                output[i] = paramsConverter[i](values[i]);

            return output;
        }

        public static object[] DeserializeParamActionValues(string paramActionId, string[] serializedValues)
        {
            var paramsConverter = ParamActionsParamsDeserializer[paramActionId];

            var output = new object[serializedValues.Length];

            for (var i = 0; i < serializedValues.Length; i++)
                output[i] = paramsConverter[i](serializedValues[i]);

            return output;
        }

        public static string SerializeBool(object? b)
        {
            if (b == null)
                return "false";

            return (bool)b ? "true" : "false";            
        }

        public static string SerializeGuid(object? id)
        {
            return id == null ? 
                string.Empty : 
                id.ToString();
        }

        public static string SerializeDouble(object? d)
        {
            if (d == null)
                return "0.0";

            return ((double)d).ToString();
        }
        public static string SerializeInt(object? i)
        {
            if (i == null)
                return "0";

            return ((int)i).ToString();
        }
        public static string SerializeString(object? s)
        {
            if (s == null)
                return string.Empty;

            return ((string)s);
        }
        public static string SerializeDateTime(object? dt)
        {
            if (dt == null)
                return new DateTime().Ticks.ToString();

            return ((DateTime)dt).Ticks.ToString();
        }

        public static bool DeserializeBool(string? b)
        {
            if (b == null || b == "false")
                return false;

            return true;
        }

        public static double DeserializeDouble(string d)
        {
            if (d == null || d == "0.0")
                return 0.0;

            return double.Parse(d);
        }
        public static int DeserializeInt(string i)
        {
            if (i == null || i == "0")
                return 0;

            return int.Parse(i);
        }
        public static string DeserializeString(string s)
        {
            if (s == null)
                return string.Empty;

            return s;
        }
        public static DateTime DeserializeDateTime(string dt)
        {
            if (dt == null || dt == "0")
                return new DateTime();

            return new DateTime().AddTicks(long.Parse(dt));
        }

        public static T DeserializeEnum<T>(string s) where T : struct, IConvertible
        {
            var i = int.Parse(s);
            return (T)(object)i;
        }

        public static Guid DeserializeGuid(string s)
        {
            return Guid.Parse(s);
        }
    }
}