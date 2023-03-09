using Common.Lib.Core.Tracking;

namespace Common.Lib.Core.Context
{
    public interface IContextFactory
    {
        bool IsServerMode { get; set; }

        IRepository<T> GetRepository<T>(IUnitOfWork? uow = null) where T : Entity, new();
        IServerRepository GetRepository(string repoTypeName);

        T NewModel<T>() where T : Entity, new();

        Entity ReconstructEntity(IEntityInfo entityInfo);

        TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new();
    }
}
