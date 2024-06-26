﻿using Common.Lib.Authentication;
using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Services;
using Common.Lib.Infrastructure;
using System.Text.Json.Serialization;

namespace Common.Lib.Core
{
    public partial class Entity
    {
        private Guid id;

        #region Persisted properties

        /// <summary>
        /// Entity unique Id
        /// </summary>
        public Guid Id 
        { 
            get => id; 
            set
            {
                id = value;
            }
        }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        #endregion

        #region Not persisted properties

        /// <summary>
        /// The unmodified Entity, used to get changes after
        /// the entty is cloned and modified
        /// </summary>
        [JsonIgnore]
        public Entity? Origin { get; set; }

        [JsonIgnore]
        public bool IsNew { get; set; } = true;

        [JsonIgnore]
        public int LastQueryNetingLevel { get; set; }

        [JsonIgnore]
        public IContextFactory? ContextFactory { get; set; }

        [JsonIgnore]
        public Func<AuthInfo?, ITraceInfo?, Task<ISaveResult>>? SaveAction { get; set; }
        [JsonIgnore]
        public Func<AuthInfo?, ITraceInfo?, Task<IDeleteResult>>? DeleteAction { get; set; }

        [JsonIgnore]
        public Func<Dictionary<Guid, Entity>, Entity> CloneAction { get; set; }

        [JsonIgnore]
        public bool IsAlreadyAssigned { get; set; }

        [JsonIgnore]
        internal ValidationResult? ValidationResult { get; set; }

        [JsonIgnore]
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
            var alreadyCloned = new Dictionary<Guid, Entity>();
            return Clone<Entity>(alreadyCloned);
        }

        public async Task<T> CloneFromRepository<T>() where T : Entity, new()
        {
            using var repoT = ContextFactory.GetRepository<T>();

            var qr1 = await repoT.FindAsync(this.Id);

            if (qr1.IsSuccess && qr1.Value != null)
            {
                var output = qr1.Value.Clone<T>([]);
                return output;
            }
            return null;
        }

        /// <summary>
        /// Method used to reproduce the entity, including itself
        /// as Origin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Clone<T>(Dictionary<Guid, Entity> alreadyCloned) where T : Entity, new()
        {
            if (alreadyCloned.ContainsKey(this.Id))
                return alreadyCloned[this.Id] as T;

            var output = new T
            {
                Origin = this,
                Id = Id,
                ContextFactory = ContextFactory,
                IsNew = false,
                CreatedOn = CreatedOn,
                UpdatedOn = UpdatedOn
            };

            alreadyCloned.Add(this.Id, output);

            return output;
        }

        public virtual async Task<T> CloneTo<T>(Dictionary<Guid, Entity> alreadyCloned) where T : Entity, new()
        {
            using var repo = ContextFactory.GetRepository<T>();
            var qr1 = await repo.FindAsync(Id);
            if (qr1.IsSuccess)
                return qr1.Value.Clone<T>(alreadyCloned);
            else
                return null;
        }

        public virtual void IncludeChildren(Dictionary<Guid, Entity> refEnts, int nestingLevel)
        {
        }

        public virtual Dictionary<Guid, Entity> GetParents()
        {
            var output = new Dictionary<Guid, Entity>();
            return output;                
        }

        public virtual void AssignChildren(QueryResult qr)
        {            
        }

        public virtual void AssignParents()
        {
        }

        public virtual void AssignParents(Dictionary<Guid, Entity> uowEntities)
        {

        }

        public virtual void AssignToParents()
        {

        }

        public virtual void UnassingFromParents()
        {

        }

        public void Do<T>(Action<T> action) where T : Entity, new()
        {
            action(this as T);
        }

        public virtual void DetachReferences()
        {

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
                this.CreatedOn = this.CreatedOn != default ? this.CreatedOn : DateTime.Now;
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

        public virtual ISaveResult Save(IUnitOfWork uow)
        {
            if (!ContextFactory.IsServerMode)
            {
                var formatValidation = StaticSaveValidation<Entity>();
                if (formatValidation != null)
                    return formatValidation;
            }

            uow.AddEntitySave(this);
            return new SaveResult<Entity>()
            {
                IsSuccess = true
            };
        }

        public virtual IDeleteResult? Delete(IUnitOfWork uow)
        {
            if (!ContextFactory.IsServerMode)
            {
                var formatValidation = StaticDeleteValidation<Entity>();
                if (formatValidation != null)
                    return formatValidation;
            }

            uow.AddEntityDelete(this);
            return new DeleteResult()
            {
                IsSuccess = true
            };
        }

        public virtual ISaveResult<T>? StaticSaveValidation<T>() where T : Entity, new()
        {
            if (this is not T)
                return new SaveResult<T>()
                {
                    IsSuccess = false,
                    Message = $"Entity of type {this.GetType().Name} cannot be saved as {typeof(T).Name}"
                };

            if (ContextFactory == null)
                throw new ContextFactoryNullException("Entity", "SaveAsync");

            //if (uow != null && !ContextFactory.IsServerMode)
            //    throw new ApplicationException("You cannot use SaveAsync with UnitOfWork in client mode. Use Save instead");

            return null;

        }

        public virtual Task<ISaveResult<T>?> DynamicSaveValidation<T>() where T : Entity, new()
        {
            return Task.FromResult(default(ISaveResult<T>?));
        }

        public virtual IDeleteResult StaticDeleteValidation<T>() where T : Entity, new()
        {
            if (ContextFactory == null)
                throw new ContextFactoryNullException("Entity", "DeleteAsync");

            //if (uow != null && !ContextFactory.IsServerMode)
            //    throw new ApplicationException("You cannot use DeleteAsync with UnitOfWork in client mode. Use Save instead");

            //using var repo = ContextFactory.GetRepository<T>() ??
            //    throw new RepositoryNotRegisteredException(typeof(T));

            return null;
        }

        public virtual Task<IDeleteResult?> DynamicDeleteValidation<T>() where T : Entity, new()
        {
            return Task.FromResult(default(IDeleteResult?));
        }

        public virtual async Task<ISaveResult<T>> SaveAsync<T>(AuthInfo? info = null, ITraceInfo? trace = null) where T : Entity, new()
        {
            var formatValidation = StaticSaveValidation<T>();

            if (ContextFactory.IsServerMode)
                formatValidation = await DynamicSaveValidation<T>();

            if (formatValidation != null && !formatValidation.IsSuccess)
                return formatValidation;

            using var repo = ContextFactory.GetRepository<T>() ??
                throw new RepositoryNotRegisteredException(typeof(T));

            var entity = this as T;
            //entity.CleanNavigationProperties();

            if (IsNew || Id == default)
            {
                var output = await repo.AddAsync(entity, info, trace);

                if (output.IsSuccess && ContextFactory.IsServerMode)
                {
                    AssignToParents();
                }
                return output;
            }
            else
            {
                return await repo.UpdateAsync(entity, info, trace);
            }            
        }

        public virtual async Task<IDeleteResult> DeleteAsync<T>(AuthInfo? info = null, ITraceInfo? trace = null) where T : Entity, new()
        {
            var formatValidation = StaticDeleteValidation<T>(); 
            
            if (ContextFactory.IsServerMode)
                formatValidation = await DynamicDeleteValidation<T>();

            if (formatValidation != null)
                return formatValidation;

            using var repo = ContextFactory.GetRepository<T>() ??
                throw new RepositoryNotRegisteredException(typeof(T));

            var output = await repo.DeleteAsync(this.Id, info);

            if (output.IsSuccess && ContextFactory.IsServerMode)
                UnassingFromParents();

            return output;
        }

        public virtual Task<List<Guid>> GetDependentEntities()
        {
            return Task.FromResult(new List<Guid>());
        }

        public virtual void SetNavigationProperties(Dictionary<string, object> ents)
        {

        }

        public virtual Dictionary<string, object> CleanNavigationProperties()
        {
            var output = new Dictionary<string, object>();
            return output;
        }

        #endregion

        #region Parametric actions

        public virtual async Task<IProcessActionResult?> RequestParametricActionAsync(string repoType, string actionName, object[] actionParams, IUnitOfWork uow, TraceInfo? traceInfo = null)
        {
            if (ContextFactory == null)
                throw new InvalidOperationException("you must get a model through the propel channels, ContextFactory is missing");

            if (ContextFactory.IsServerMode)
                return default;

            var svcInvoker = ContextFactory.Resolve<IServiceInvoker>();

            var paramsCarrierFactory = ContextFactory.Resolve<IParamsCarrierFactory>();

            var userId = Guid.NewGuid(); //todo: implement user auth
            var userToken = string.Empty;//todo: implement user auth
            var userEmail = string.Empty;
            var paramsCarrier = paramsCarrierFactory.CreateParametricActionParams(
                                                            userId,
                                                            userToken,
                                                            userEmail,
                                                            traceInfo,
                                                            DateTime.Now,
                                                            repoType,
                                                            this.Id,
                                                            actionName,
                                                            actionParams);

            var result = await svcInvoker.RequestParametricActionAsync(paramsCarrier);
            return result;
        }

        public virtual async Task<IProcessActionResult?> ProcessActionAsync(string actionId, object[] actionParams, IUnitOfWork uow)
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

        public static Dictionary<int, ValueTypes> MetadataMaps { get; set; } = new()
        {
            { EntityMetadata.Id, ValueTypes.Guid },
            { EntityMetadata.CreatedOn, ValueTypes.DateTime },
            { EntityMetadata.UpdatedOn, ValueTypes.DateTime }
        };

        public static Dictionary<int, string> MetadataNames { get; set; } = new()
        {
            { EntityMetadata.Id, "Entity.Id" },
            { EntityMetadata.CreatedOn, "Entity.CreatedOn" },
            { EntityMetadata.UpdatedOn, "Entity.UpdatedOn"  }
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