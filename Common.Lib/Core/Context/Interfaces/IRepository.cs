using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public interface IRepository : IContextElement
    {
        IWorkflowManager WorkflowManager { get; }
    }

    public interface IRepository<T> : ICrudHandler<T>, IQueryHandler<T>, IRepository where T : Entity, new()
    {
        IQueryAggregator<T> DeclareChildrenPolicy(int n);

        Task<QueryResult<List<T>>> ToListAsync();
    }

}
