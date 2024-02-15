using Common.Lib.Core.Metadata;
using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public class ContextFactory : IContextFactory
    {
        public string RepositoryAssemblyName { get; set; } = string.Empty;
        public bool IsServerMode { get; set; }

        protected readonly IServiceProvider ServiceProvider;
        public IRepository<T> GetRepository<T>() where T : Entity, new()
        {
            //if (uow != null && !IsServerMode)
            //    throw new ArgumentException("You cannot instantiate a UoWRepository in Client Mode");

            //if (uow == null)
                return (IRepository<T>)ServiceProvider.GetService(typeof(IRepository<T>));

            //var output = (IUoWRepository<T>)ServiceProvider.GetService(typeof(IUoWRepository<T>));
            //output.UnitOfWork = uow;

            //return output;
        }

        public IBusinessService GetBusinessService(string serviceName)
        {
            var svc = MetadataHandler.BusinessServicesConstructors[serviceName]();
            svc.ContextFactory = this;
            return svc;
        }

        public T Resolve<T>()
        {
            var output = ServiceProvider.GetService(typeof(T));

            if(output is IContextElement)
            {
                ((IContextElement)output).ContextFactory = this;
            }

            if (output == null)
                throw new ArgumentException($"{typeof(T).FullName} is not registered in the ContextFactory");

            return (T)output;
        }

        public IServerRepository GetRepository(string modelTypeName)
        {
            var repoTypeName = $"Common.Lib.Core.Context.IRepository`1[[{modelTypeName}, {RepositoryAssemblyName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]";

            var output = ServiceProvider.GetService(Type.GetType(repoTypeName));
            return (IServerRepository)output;
        }


        public T NewModel<T>() where T : Entity, new()
        {
            return new T()
            {
                ContextFactory = this,
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };
        }

        public Entity ReconstructEntity(IEntityInfo entityInfo)
        {
            if (entityInfo == null)
                return null;

            var entity = MetadataHandler.ModelsConstructors[entityInfo.EntityModelType]();

            entity.Id = entityInfo.EntityId;
            entity.ContextFactory = this;

            var changes = entityInfo.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();
            entity.ApplyChanges(changes);

            entity.AssignParents();

            return entity;
        }

        public async Task<QueryResult<Entity>> ReconstructAndUpdateEntity(IEntityInfo entityInfo)
        {
            using var repo = GetRepository(entityInfo.EntityModelType);

            var qre = await repo.FindAsync(entityInfo.EntityId);

            if (!qre.IsSuccess)
                return qre;

            var entity = qre.Value.CloneAction();

            entity.Id = entityInfo.EntityId;
            entity.ContextFactory = this;

            var changes = entityInfo.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();
            entity.ApplyChanges(changes);
            return new QueryResult<Entity>() { IsSuccess = true, Value = entity };
        }

        public TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new()
        {
            return (TEntity)ReconstructEntity(entityInfo);
        }

        public IDbSet<T> GetDbSet<T>() where T : Entity, new()
        {
            return (IDbSet<T>)ServiceProvider.GetService(typeof(IDbSet<T>));
        }

        public void Dispose()
        {
        }

        public ContextFactory(IServiceProvider serviceProvider, string repositoryAssemblyName)
        {
            ServiceProvider = serviceProvider;
            RepositoryAssemblyName = repositoryAssemblyName;
        }
    }
}
