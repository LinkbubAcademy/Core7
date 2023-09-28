using Common.Lib.Core.Tracking;
using Common.Lib.Infrastructure.Actions;
using System;

namespace Common.Lib.Core.Context
{
    public interface IContextFactory
    {
        public static Task<QueryResult<T>> GetError<T>(bool isContextFactoryNull)
        {
            return Task.FromResult(new QueryResult<T>()
            {
                IsSuccess = false,
                Message = isContextFactoryNull ?
                                        "ContextFactory is null. Use ContextFactory to create a model" :
                                        "Person Repository is not injected"
            });
        }

        bool IsServerMode { get; set; }

        IRepository<T> GetRepository<T>(IUnitOfWork? uow = null) where T : Entity, new();
        IServerRepository GetRepository(string repoTypeName);

        T NewModel<T>() where T : Entity, new();

        Entity ReconstructEntity(IEntityInfo entityInfo);

        TEntity ReconstructEntity<TEntity>(IEntityInfo entityInfo) where TEntity : Entity, new();

        T Resolve<T>();
    }
}
