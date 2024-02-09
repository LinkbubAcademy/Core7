using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public interface IQueryHandler<T> where T : Entity, new()
    {
        Task<QueryResult<T>> FindAsync(Guid id);

        QueryResult<T> Find(Guid id);

        IQueryAggregator<T> Where(params IQueryExpression[] expressions);
        IQueryAggregator<T> OrderBy<TValue>(IPropertySelector<TValue> properySelector);
        IQueryAggregator<T> OrderByDesc<TValue>(IPropertySelector<TValue> properySelector);

        Task<QueryResult<T>> FirstOrDefaultAsync(params IQueryExpression[] expressions);
        Task<QueryResult<T>> LastOrDefaultAsync(params IQueryExpression[] expressions);
        
        Task<QueryResult<List<TValue>>> SelectAsync<TValue>(IPropertySelector<TValue> propertySelector);
        Task<QueryResult<List<TValue>>> DistinctAsync<TValue>(IPropertySelector<TValue> propertySelector);

        #region Logical operations

        Task<QueryResult<bool>> AnyAsync(params IQueryExpression[] expressions);
        Task<QueryResult<bool>> AllAsync(params IQueryExpression[] expressions);
        Task<QueryResult<bool>> NoneAsync(params IQueryExpression[] expressions);

        #endregion

        #region Arithmetic operations

        Task<QueryResult<int>> CountAsync(params IQueryExpression[] expressions);
        Task<QueryResult<int>> SumAsync(IPropertySelector<int> propertySelector);
        Task<QueryResult<double>> SumAsync(IPropertySelector<double> propertySelector);
        Task<QueryResult<int>> MaxAsync(IPropertySelector<int> propertySelector);
        Task<QueryResult<double>> MaxAsync(IPropertySelector<double> propertySelector);
        Task<QueryResult<DateTime>> MaxAsync(IPropertySelector<DateTime> propertySelector);
        Task<QueryResult<int>> MinAsync(IPropertySelector<int> propertySelector);
        Task<QueryResult<double>> MinAsync(IPropertySelector<double> propertySelector);
        Task<QueryResult<DateTime>> MinAsync(IPropertySelector<DateTime> propertySelector);
        Task<QueryResult<double>> AvgAsync(IPropertySelector<int> propertySelector);
        Task<QueryResult<double>> AvgAsync(IPropertySelector<double> propertySelector);

        #endregion

    }
}
