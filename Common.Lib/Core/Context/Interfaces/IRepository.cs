﻿using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public interface IRepository : IDisposable
    {
        IContextFactory ContextFactory { get; set; }
        IWorkflowManager WorkflowManager { get; }
    }

    public interface IRepository<T> : ICrudHandler<T>, IQueryHandler<T>, IRepository where T : Entity, new()
    {
        IQueryAggregator<T> DeclareChildrenPolicy(int n);

        Task<QueryResult<List<T>>> ToListAsync();
    }

}
