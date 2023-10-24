using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public class ServerUnitOfWork : IContextFactory, IUnitOfWork
    {
        public IContextFactory ContextFactory { get; set; }
        public bool IsServerMode
        {
            get
            {
                return ContextFactory.IsServerMode;
            }
            set
            {
                ContextFactory.IsServerMode = value;
            }
        }

        public ServerUnitOfWork(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        public void AddEntitySave(Entity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = null)
        {            
            var output = new QueryResult();

            var entities = new List<object>();

            var repos = new Dictionary<string,  IServerRepository>();

            foreach (var action in actions)
            {
                var type = action.Change.EntityModelType;

                if (!repos.ContainsKey(type))
                    repos.Add(type, ContextFactory.GetRepository(type));

                IServerRepository currentRepo = repos[type];

                var change = action.Change;

                switch (action.ActionInfoType)
                {
                    case ActionInfoTypes.Save:

                        if (change.IsNew || change.EntityId == default)
                        {
                            var qrEexistingEntity = await currentRepo.FindAsync(change.EntityId);

                            if (qrEexistingEntity.Value == null)
                            {
                                var entityToAdd = ContextFactory.ReconstructEntity(change);
                                if (entityToAdd.Id == default)
                                    entityToAdd.Id = change.EntityId != default ? change.EntityId : Guid.NewGuid();

                                entityToAdd.SaveAction();
                            }
                            else
                            {
                                //todo: handle Entity exists error
                            }
                        }
                        else
                        {
                            var qrEexistingEntity = await currentRepo.FindAsync(change.EntityId);

                            if (qrEexistingEntity.IsSuccess && qrEexistingEntity.Value != null)
                            {
                                var entityToUpdate = qrEexistingEntity.Value.Clone();
                                entityToUpdate.ApplyChanges(change.GetChangeUnits());
                            }
                            else
                            {
                                //todo: handle Entity exists error
                            }

                        }

                        break;
                    default:
                    case ActionInfoTypes.Delete:
                        //Todo
                        throw new NotImplementedException();
                }

                //var result = await EfDbContext.SaveChangesAsync();
                //if (result == 0)
                //    return false;
                //else
                //{
                //    foreach (var notif in onSavedNotifications)
                //        notif();
                //}
            }

            return null;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void AddEntityDelete(Entity entity)
        {
            throw new NotImplementedException();
        }

        public IRepository<T> GetRepository<T>() where T : Entity, new()
        {
            var output = ContextFactory.GetRepository<T>();
            return output;
        }

        public IServerRepository GetRepository(string repoTypeName)
        {
            throw new NotImplementedException();
        }

        public T NewModel<T>() where T : Entity, new()
        {
            return ContextFactory.NewModel<T>();
        }

        public Entity ReconstructEntity(IEntityInfo entityInfo)
        {
            throw new NotImplementedException();
        }

        public Entity ReconstructAndUpdateEntity(IEntityInfo entityInfo)
        {
            throw new NotImplementedException();
        }

        public TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            return ContextFactory.Resolve<T>();
        }
    }
}
