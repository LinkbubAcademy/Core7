using Common.Lib.Core.Tracking;
using Common.Lib.DataAccess.EFCore;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public class ServerUnitOfWork : UnitOfWork, IContextFactory
    {
        public IContextFactory ContextFactory { get; set; }
        public IDbSetProvider DbSetProvider { get; set; }

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

        public ServerUnitOfWork(IContextFactory contextFactory, IDbSetProvider dbSetProvider)
        {
            ContextFactory = contextFactory;
            DbSetProvider = dbSetProvider;
        }

        public override async Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = null)
        {
            if (actions != null)
                UowActions = actions.ToList();
                
            var output = new QueryResult();

            var entities = new List<object>();

            var repos = new Dictionary<string,  IServerRepository>();

            foreach (var action in UowActions)
            {
                var type = action.Change.EntityModelType;

                if (!repos.ContainsKey(type))
                    repos.Add(type, this.GetRepository(type));

                IServerRepository currentRepo = repos[type];

                var change = action.Change;

                switch (action.ActionInfoType)
                {
                    case ActionInfoTypes.Save:

                        if (change.IsNew || change.EntityId == default)
                        {
                            var qrExistingEntity = await currentRepo.FindAsync(change.EntityId);

                            if (qrExistingEntity.Value == null)
                            {
                                var entityToAdd = this.ReconstructEntity(change);
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
                            var ue = this.ReconstructAndUpdateEntity(change);

                            if (ue != null)
                            {
                                ue.DetachRefernces();
                                ue.SaveAction();
                            }
                            else
                            {
                                //todo: handle Entity exists error
                            }
                        }

                        break;
                    case ActionInfoTypes.Delete:

                        var qrEntityToRemove = await currentRepo.FindAsync(change.EntityId);
                        if (qrEntityToRemove.IsSuccess && qrEntityToRemove.Value != null)
                        {
                            var entityToRemove = qrEntityToRemove.Value;
                            entityToRemove.ContextFactory = this;
                            entityToRemove.DeleteAction();
                        }
                        break;
                }


                //else
                //{
                //    foreach (var notif in onSavedNotifications)
                //        notif();
                //}
            }

            var result = await DbSetProvider.SaveChangesAsync();

            if (result > 0)
            {
                foreach(var dbSet in DbSets.Values)
                    dbSet.UpdateCache();

                return new ActionResult()
                {
                    IsSuccess = true,
                    Message = $"total changes applied {result}"
                };
            }

            return new ActionResult()
            {
                IsSuccess = false,
                Message = "no changes were applied"
            };
        }

        #region Encapsulate IContextFactory
        public IRepository<T> GetRepository<T>() where T : Entity, new()
        {
            var output = ContextFactory.GetRepository<T>();
            output.ContextFactory = this;

            return output;
        }

        public IServerRepository GetRepository(string repoTypeName)
        {
            var output = ContextFactory.GetRepository(repoTypeName);
            output.ContextFactory = this;
            return output;
        }

        public T NewModel<T>() where T : Entity, new()
        {
            var output = ContextFactory.NewModel<T>();
            output.ContextFactory = this;

            return output;
        }

        public Entity ReconstructEntity(IEntityInfo entityInfo)
        {
            var output = ContextFactory.ReconstructEntity(entityInfo);
            output.ContextFactory = this;

            return output;
        }

        public Entity ReconstructAndUpdateEntity(IEntityInfo entityInfo)
        {
            var dbContext = ContextFactory.Resolve<CommonEfDbContext>();
            var existingEntity = dbContext.FindEntityFromDb(entityInfo.EntityModelType, entityInfo.EntityId);

            existingEntity.IsNew = false;
            existingEntity.ApplyChanges(entityInfo.GetChangeUnits());
            existingEntity.ContextFactory = this;
            ////var output = ContextFactory.ReconstructAndUpdateEntity(entityInfo);
            //output.ContextFactory = this;

            //return existingEntity;
            return existingEntity;
        }

        public TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new()
        {
            var output = ContextFactory.ReconstructEntity<TEntity>(entityInfo);
            output.ContextFactory = this;

            return output;
        }

        public T Resolve<T>()
        {
            return ContextFactory.Resolve<T>();
        }

        Dictionary<Type, IUnitOfWorkDbSet> DbSets { get; set; } = new();

        public IDbSet<T> GetDbSet<T>() where T : Entity, new()
        {
            if (!DbSets.ContainsKey(typeof(T)))
            {
                var dbSet = new UnitOfWorkDbSet<T>(DbSetProvider.ResolveDbSet<T>());
                DbSets.Add(typeof(T), dbSet);
            }

            return (UnitOfWorkDbSet<T>)DbSets[typeof(T)];
        }

        #endregion
    }
}