﻿using Common.Lib.Core.Tracking;
using Common.Lib.DataAccess.EFCore;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public class ServerUnitOfWork : UnitOfWork, IContextFactory
    {
        public ServerUnitOfWork ParentUoW { get; set; }

        public override TimeInfoLog TimeInfoLog
        {
            get
            {
                if (ParentUoW != null)
                    return ParentUoW.TimeInfoLog;
                else
                    return timeInfoLog;
            }
            set
            {
                timeInfoLog = value;
            }
        }
        TimeInfoLog timeInfoLog;

        public IContextFactory ContextFactory { get; set; }
        public IDbSetProvider DbSetProvider { get; set; }

        public Dictionary<Guid, Entity> EntitiesInUoW
        {
            get
            {
                return ParentUoW != null ? ParentUoW.EntitiesInUoW : entitiesInUoW;
            }
            set
            {
                entitiesInUoW = value;
            }
        }

        Dictionary<Guid, Entity> entitiesInUoW = new Dictionary<Guid, Entity>();

        public UowNotificationHandler NotificationHandler { get; set; } = new UowNotificationHandler();

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

            var repos = new Dictionary<string,  IServerRepository>();

            foreach (var action in UowActions)
            {
                var type = action.Change.EntityModelType;

                //TimeInfoLog?.AddTimeEntry($"UOW_Start_Action_EntityType:{type}:");

                if (!repos.ContainsKey(type))
                    repos.Add(type, this.GetRepository(type));

                IServerRepository currentRepo = repos[type];

                var change = action.Change;

                switch (action.ActionInfoType)
                {
                    case ActionInfoTypes.Save:

                        if (change.IsNew || change.EntityId == default)
                        {
                            TimeInfoLog?.AddTimeEntry($"UOW_start_Add____{type}_Id:{change.EntityId}:");

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


                            TimeInfoLog?.AddTimeEntry($"UOW_end___Add____{type}_Id:{change.EntityId}:");
                        }
                        else
                        {
                            TimeInfoLog?.AddTimeEntry($"UOW_start_Update_{type}_Id:{change.EntityId}");
                            var qr = await this.ReconstructAndUpdateEntity(change);

                            if (qr.IsSuccess && qr.Value != null)
                            {
                                var ue = qr.Value;
                                if (EntitiesInUoW.ContainsKey(ue.Id))
                                    EntitiesInUoW[ue.Id] = ue;
                                else
                                    EntitiesInUoW.Add(ue.Id, ue);

                                //ue.AssignParents(entities);
                                ue.SaveAction();
                            }
                            else
                            {
                                //todo: handle Entity exists error
                            }

                            TimeInfoLog?.AddTimeEntry($"UOW_end___Update_{type}_Id:{change.EntityId}:");
                        }

                        break;
                    case ActionInfoTypes.Delete:

                        TimeInfoLog?.AddTimeEntry($"UOW_start_Delete_{type}_Id:{change.EntityId}:");
                        var qrEntityToRemove = await currentRepo.FindAsync(change.EntityId);
                        if (qrEntityToRemove.IsSuccess && qrEntityToRemove.Value != null)
                        {
                            var entityToRemove = qrEntityToRemove.Value;
                            entityToRemove.ContextFactory = this;
                            entityToRemove.DeleteAction();
                        }
                        TimeInfoLog?.AddTimeEntry($"UOW_end___Delete_{type}_Id:{change.EntityId}:");
                        break;
                }


            }

            TimeInfoLog?.AddTimeEntry("UOW data to persist prepared");
            var result = await DbSetProvider.SaveChangesAsync();
            TimeInfoLog?.AddTimeEntry("UOW persistance action");

            if (result > 0)
            {
                using var uowAlt = (ServerUnitOfWork)ContextFactory.Resolve<IUnitOfWork>();
                uowAlt.ParentUoW = this.ParentUoW != null ? this.ParentUoW : this;

                var saveAltRequired = false;

                foreach(var pair in EntitiesInUoW)
                {
                    var id = pair.Key;
                    var entity = pair.Value;
                    var type = entity.GetType();

                    if (DbSets.ContainsKey(type))
                    {
                        var dbSet = DbSets[type];
                        var efromdb = dbSet.Find(type, id);

                        if (efromdb == null)
                        {
                            NotificationHandler.DeleteNotification(id);
                            entity.Save(uowAlt);
                            saveAltRequired = true;
                        }
                    }
                }
                           
                if (saveAltRequired)
                {
                    var srAlt = await uowAlt.CommitAsync();
                }


                TimeInfoLog?.AddTimeEntry("UOW caching info start");
                foreach (var dbSet in DbSets.Values.ToList())
                    await dbSet.UpdateCache();
                TimeInfoLog?.AddTimeEntry("UOW caching info end");

                if (ParentUoW == null)
                {

                    TimeInfoLog?.AddTimeEntry("UOW handle notification start");
                    NotificationHandler.HandlerAllNotifications();
                    TimeInfoLog?.AddTimeEntry("UOW handle notification end");
                }

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

        public IBusinessService GetBusinessService(string serviceName)
        {
            var output = ContextFactory.GetBusinessService(serviceName);
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
            output.AssignParents(EntitiesInUoW);

            if (EntitiesInUoW.ContainsKey(output.Id))
                EntitiesInUoW[output.Id] = output;
            else
                EntitiesInUoW.Add(output.Id, output);

            return output;
        }

        public async Task<QueryResult<Entity>> ReconstructAndUpdateEntity(IEntityInfo entityInfo)
        {
            var dbContext = ContextFactory.Resolve<CommonEfDbContext>();
            var existingEntity = dbContext.FindEntityFromDb(entityInfo.EntityModelType, entityInfo.EntityId);

            existingEntity.IsNew = false;
            existingEntity.ApplyChanges(entityInfo.GetChangeUnits());
            existingEntity.ContextFactory = this;
            existingEntity.AssignParents(EntitiesInUoW);

            return new QueryResult<Entity>() { IsSuccess = existingEntity != null, Value = existingEntity };
        }

        public async Task<QueryResult<Entity>> ReconstructAndUpdateEntity(IEntityInfo entityInfo, Dictionary<Guid, Entity> entitiesInUoW)
        {
            EntitiesInUoW = entitiesInUoW;
            return await ReconstructAndUpdateEntity(entityInfo);
        }

        public TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new()
        {
            var output = ContextFactory.ReconstructEntity<TEntity>(entityInfo);
            output.ContextFactory = this;

            return output;
        }

        public T Resolve<T>()
        {
            if (typeof(T) == typeof(INotificationHandler))
                return (T)(object)NotificationHandler;

            return ContextFactory.Resolve<T>();
        }

        Dictionary<Type, IUnitOfWorkDbSet> DbSets { get; set; } = new();

        public IDbSet<T> GetDbSet<T>() where T : Entity, new()
        {
            if (!DbSets.ContainsKey(typeof(T)))
            {
                var dbSet = new UnitOfWorkDbSet<T>((CommonEfDbContext)DbSetProvider, DbSetProvider.ResolveDbSet<T>());
                DbSets.Add(typeof(T), dbSet);
            }

            return (UnitOfWorkDbSet<T>)DbSets[typeof(T)];
        }

        #endregion
    }
}