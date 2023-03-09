using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public interface IQueryAggregator<T> : IQueryHandler<T> where T : Entity, new()
    {
        Task<QueryResult<List<T>>> ToListAsync();
    }
}
