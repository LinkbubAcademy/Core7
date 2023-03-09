using Common.Lib.Authentication;
using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure.Actions;

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
            changes = changes.OrderBy(x => x.MetdataId).ToList();

            foreach (var change in changes)
            {
                switch (change.MetdataId)
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
                        return changes.Where(x => x.MetdataId > EntityMetadata.Last).ToList();
                }
            }

            return changes.Where(x => x.MetdataId > EntityMetadata.Last).ToList();
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
                var updateErrors = await ValidateNew();
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

        public static ModelRolesMap EntityRolesMap
        {
            get
            {
                return _entityRolesMap ??= new ModelRolesMap();
            }
        }
        static ModelRolesMap? _entityRolesMap;


        public virtual ModelRolesMap GetModelRolesMap()
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
    }
}