namespace Common.Lib.Core.Context
{
    public interface IRepository : IDisposable
    {
        IWorkflowManager WorkflowManager { get; }
    }

    public interface IRepository<T> : ICrudHandler<T>, IQueryHandler<T>, IRepository where T : Entity, new()
    {
        IQueryAggregator<T> DeclareChildrenPolicy(int n);
    }

}
