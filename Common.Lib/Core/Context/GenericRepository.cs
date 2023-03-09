using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;
using System.Linq.Expressions;

namespace Common.Lib.Core.Context
{
    public class GenericRepository<T> : IRepository<T> where T : Entity, new()
    {
        public virtual IWorkflowManager WorkflowManager
        {
            get
            {
                return _workflowManager;
            }
        }

        readonly IWorkflowManager _workflowManager;


        public virtual IDbSet<T> DbSet
        {
            get
            {
                return _dbSet;
            }
        }

        readonly IDbSet<T> _dbSet;

        public GenericRepository(IDbSet<T> dbSet, IWorkflowManager workflowManager)
        {
            _workflowManager = workflowManager;
            _dbSet = dbSet;
        }

        public IQueryAggregator<T> DeclareChildrenPolicy(int n)
        {
            DbSet.NestingLevel = n < 0 ? 0 : n;
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator;
        }

        #region CRUD operations
        public virtual Task<ActionResult> AddAsync(T entity)
        {
            if (entity.Id == default)
                entity.Id = Guid.NewGuid();

            var output = DbSet.AddAsync(entity);

            return output;
        }

        public virtual Task<ActionResult> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ActionResult> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<T>> FindAsync(Guid id)
        {
            return DbSet.FindAsync(id);
        }


        #endregion

        #region Retrieving operations

        public IQueryAggregator<T> Where(params IQueryExpression[] expressions)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.Where(expressions);
        }
        public IQueryAggregator<T> OrderBy<TValue>(IPropertySelector<TValue> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.OrderBy(propertySelector);
        }
        public IQueryAggregator<T> OrderByDesc<TValue>(IPropertySelector<TValue> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.OrderByDesc(propertySelector);
        }

        public Task<QueryResult<T>> FirstOrDefaultAsync(params IQueryExpression[] expressions)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.FirstOrDefaultAsync(expressions);
        }
        public Task<QueryResult<T>> LastOrDefaultAsync(params IQueryExpression[] expressions)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.FirstOrDefaultAsync(expressions);
        }

        public Task<QueryResult<List<TValue>>> SelectAsync<TValue>(IPropertySelector<TValue> properySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.SelectAsync(properySelector);
        }

        public Task<QueryResult<List<TValue>>> DistinctAsync<TValue>(IPropertySelector<TValue> properySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.DistinctAsync(properySelector);
        }

        #endregion

        #region Logical operations

        public Task<QueryResult<bool>> AnyAsync(params IQueryExpression[] expressions)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.AnyAsync(expressions);
        }

        public Task<QueryResult<bool>> AllAsync(params IQueryExpression[] expressions)
        {

            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.AllAsync(expressions);
        }
        public Task<QueryResult<bool>> NoneAsync(params IQueryExpression[] expressions)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.NoneAsync(expressions);

        }

        #endregion

        #region Arithmetic operations

        public Task<QueryResult<int>> CountAsync(params IQueryExpression[] expressions)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.CountAsync(expressions);
        }

        public Task<QueryResult<int>> SumAsync(IPropertySelector<int> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);            
            return queryAggregator.SumAsync(propertySelector);
        }

        public Task<QueryResult<double>> SumAsync(IPropertySelector<double> propertySelector)
        {

            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.SumAsync(propertySelector);
        }

        public Task<QueryResult<int>> MaxAsync(IPropertySelector<int> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.MaxAsync(propertySelector);
        }

        public Task<QueryResult<DateTime>> MaxAsync(IPropertySelector<DateTime> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.MaxAsync(propertySelector);
        }

        public Task<QueryResult<double>> MaxAsync(IPropertySelector<double> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.MaxAsync(propertySelector);
        }
        public Task<QueryResult<int>> MinAsync(IPropertySelector<int> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.MinAsync(propertySelector);
        }

        public Task<QueryResult<double>> MinAsync(IPropertySelector<double> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.MinAsync(propertySelector);
        }
        public Task<QueryResult<DateTime>> MinAsync(IPropertySelector<DateTime> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.MinAsync(propertySelector);
        }
        public Task<QueryResult<double>> AvgAsync(IPropertySelector<int> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.AvgAsync(propertySelector);
        }

        public Task<QueryResult<double>> AvgAsync(IPropertySelector<double> propertySelector)
        {
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator.AvgAsync(propertySelector);
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
