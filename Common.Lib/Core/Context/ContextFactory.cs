using Common.Lib.Core.Metadata;
using Common.Lib.Core.Tracking;

namespace Common.Lib.Core.Context
{
    public class ContextFactory : IContextFactory
    {
        public string RepositoryAssemblyName { get; set; } = string.Empty;
        public bool IsServerMode { get; set; }

        protected readonly IServiceProvider ServiceProvider;
        public IRepository<T> GetRepository<T>(IUnitOfWork? uow = null) where T : Entity, new()
        {
            if (uow != null && !IsServerMode)
                throw new ArgumentException("You cannot instantiate a UoWRepository in Client Mode");

            if (uow == null)
                return (IRepository<T>)ServiceProvider.GetService(typeof(IRepository<T>));

            var output = (IUoWRepository<T>)ServiceProvider.GetService(typeof(IUoWRepository<T>));
            output.UnitOfWork = uow;

            return output;
        }

        public T Resolve<T>()
        {
            var output = ServiceProvider.GetService(typeof(T));

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
            var entity = MetadataHandler.ModelsConstructors[entityInfo.EntityModelType]();

            entity.Id = entityInfo.EntityId;
            entity.ContextFactory = this;

            var changes = entityInfo.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();

            entity.ApplyChanges(changes);
            return entity;
        }

        public Entity ReconstructAndUpdateEntity(IEntityInfo entityInfo)
        {
            using var repo = GetRepository(entityInfo.EntityModelType);
            var entity = repo.FindAsync(entityInfo.EntityId).Result.Value;

            entity.Id = entityInfo.EntityId;
            entity.ContextFactory = this;

            var changes = entityInfo.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();
            entity.ApplyChanges(changes);
            return entity;
        }

        public TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new()
        {
            return (TEntity)ReconstructEntity(entityInfo);
        }

        public ContextFactory(IServiceProvider serviceProvider, string repositoryAssemblyName)
        {
            ServiceProvider = serviceProvider;
            RepositoryAssemblyName = repositoryAssemblyName;
        }
    }
}
