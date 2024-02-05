using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.Lib.Core.Context
{
    public class GenericRepository<T> : IRepository<T> where T : Entity, new()
    {
        public IContextFactory ContextFactory { get; set; }
        public virtual IWorkflowManager WorkflowManager
        {
            get
            {
                return _workflowManager;
            }
        }

        public ILogHandler<T> LogHandler { get; set; }

        readonly IWorkflowManager _workflowManager;


        public virtual IDbSet<T> DbSet
        {
            get
            {
                if (_dbSet == null)
                    _dbSet = ContextFactory.GetDbSet<T>();

                return _dbSet;
            }
        }

        IDbSet<T> _dbSet;

        public GenericRepository(IWorkflowManager workflowManager, IContextFactory contextFactory, ILogHandler<T> logHandler = null)
        {
            _workflowManager = workflowManager;
            ContextFactory = contextFactory;
            LogHandler = logHandler;
        }

        public IQueryAggregator<T> DeclareChildrenPolicy(int n)
        {
            DbSet.NestingLevel = n < 0 ? 0 : n;
            var queryAggregator = new QueryAggregator<T>(DbSet);
            return queryAggregator;
        }

        #region CRUD operations
        public virtual Task<ISaveResult<T>> AddAsync(T entity)
        {
            if (entity.Id == default)
                entity.Id = Guid.NewGuid();

            var output = DbSet.AddAsync(entity);

            LogHandler?.Log(entity);

            return output;
        }
            
        public virtual Task<IDeleteResult> DeleteAsync(Guid id)
        {
            var output = DbSet.DeleteAsync(id);
            return output;
        }

        public virtual Task<ISaveResult<T>> UpdateAsync(T entity)
        {
            var output = DbSet.UpdateAsync(entity);
            return output;
        }

        public Task<QueryResult<T>> FindAsync(Guid id)
        {
            return DbSet.FindAsync(id);
        }


        #endregion

        #region Retrieving operations

        public async Task<QueryResult<List<T>>> ToListAsync()
        {
            Log.WriteLine("GenericRepository ToListAsync 1");
            return await Where(EmptyExpression.Create()).ToListAsync();
        }

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
