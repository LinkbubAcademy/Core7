using Common.Lib.Core;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services.ParamsCarriers;

namespace Common.Lib.Services
{
    public interface IServiceInvoker
    {
        string BaseUri { get; set; }

        Task<IProcessActionResult> RequestParametricActionAsync(IParametricActionParamsCarrier paramsCarrier);
        Task<IProcessActionResult> RequestServiceActionAsync(IServiceActionParamsCarrier paramsCarrier);
        Task<ISaveResult<TEntity>> AddNewEntityRequestAsync<TEntity>(ISaveEntityParamsCarrier paramsCarrier) where TEntity : Entity, new();
        Task<ISaveResult<TEntity>> UpdateEntityRequestAsync<TEntity>(ISaveEntityParamsCarrier paramsCarrier) where TEntity : Entity, new();
        Task<IDeleteResult> DeleteEntityRequestAsync(IDeleteEntityParamsCarrier paramsCarrier);
        Task<QueryResult<TEntity>> QueryRepositoryForEntity<TEntity>(IQueryRepositoryParamsCarrier paramsCarrier) where TEntity : Entity, new();

        Task<IActionResult> CommitUnitOfWorkAsync(IUnitOfWorkParamsCarrier paramsCarrier);
        Task<QueryResult<List<TEntity>>> QueryRepositoryForEntities<TEntity>(IQueryRepositoryParamsCarrier paramsCarrier) where TEntity : Entity, new();
        
        Task<QueryResult<bool>> QueryRepositoryForBool(IQueryRepositoryParamsCarrier paramsCarrier);
        Task<QueryResult<int>> QueryRepositoryForInt(IQueryRepositoryParamsCarrier paramsCarrier);
        Task<QueryResult<double>> QueryRepositoryForDouble(IQueryRepositoryParamsCarrier paramsCarrier);
        Task<QueryResult<string>> QueryRepositoryForString(IQueryRepositoryParamsCarrier paramsCarrier);
        Task<QueryResult<DateTime>> QueryRepositoryForDateTime(IQueryRepositoryParamsCarrier paramsCarrier);
        Task<QueryResult<Guid>> QueryRepositoryForGuid(IQueryRepositoryParamsCarrier paramsCarrier);
        
        Task<QueryResult<List<TValue>>> QueryRepositoryForValues<TValue>(IQueryRepositoryParamsCarrier paramsCarrier);
    }
}
