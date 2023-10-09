using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    public class QueryAggregator<T> : IQueryAggregator<T> where T : Entity, new()
    {
        IDbSet<T> DbSet { get; set; }

        List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations { get; set; } = new List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>>();

        QueryTypes LastQuery { get; set; }

        public QueryAggregator(IDbSet<T> dbSet)
        {
            DbSet = dbSet;
        }

        public Task<QueryResult<T>> FindAsync(Guid id)
        {
            return DbSet.FindAsync(id);
        }

        public Task<QueryResult<T>> FirstOrDefaultAsync(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.FirstOrDefault);
            return DbSet.GetEntityAsync(Operations);
        }

        public Task<QueryResult<T>> LastOrDefaultAsync(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.LastOrDefault);
            return DbSet.GetEntityAsync(Operations);
        }
        
        public IQueryAggregator<T> Where(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.Where);
            return this;
        }

        public IQueryAggregator<T> OrderBy<TValue>(IPropertySelector<TValue> propertySelector)
        {
            AddPropertyOperation<TValue>(propertySelector, QueryTypes.OrderBy);
            return this;
        }

        public IQueryAggregator<T> OrderByDesc<TValue>(IPropertySelector<TValue> propertySelector)
        {
            AddPropertyOperation<TValue>(propertySelector, QueryTypes.OrderByDesc);
            return this;
        }
        public Task<QueryResult<List<TValue>>> SelectAsync<TValue>(IPropertySelector<TValue> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Select);
            return DbSet.GetValuesAsync<TValue>(Operations);
        }

        public Task<QueryResult<List<TValue>>> DistinctAsync<TValue>(IPropertySelector<TValue> propertySelector)
        {
            AddPropertyOperation<TValue>(propertySelector, QueryTypes.Distinct);
            return DbSet.GetValuesAsync<TValue>(Operations);
        }

        #region Logical operations

        public Task<QueryResult<bool>> AnyAsync(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.Any);
            return DbSet.GetBoolValueAsync(Operations);
        }

        public Task<QueryResult<bool>> AllAsync(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.All);
            return DbSet.GetBoolValueAsync(Operations);
        }
        public Task<QueryResult<bool>> NoneAsync(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.None);
            return DbSet.GetBoolValueAsync(Operations);

        }

        #endregion

        #region Arithmetic operations

        public Task<QueryResult<int>> CountAsync(params IQueryExpression[] expressions)
        {
            AddBoolOperation(expressions, QueryTypes.Count);
            return DbSet.GetIntValueAsync(Operations);
        }

        public Task<QueryResult<int>> SumAsync(IPropertySelector<int> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Sum);
            return DbSet.GetIntValueAsync(Operations);
        }

        public Task<QueryResult<double>> SumAsync(IPropertySelector<double> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Sum);
            return DbSet.GetDoubleValueAsync(Operations);
        }
        public Task<QueryResult<int>> MaxAsync(IPropertySelector<int> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Max);
            return DbSet.GetIntValueAsync(Operations);
        }

        public Task<QueryResult<DateTime>> MaxAsync(IPropertySelector<DateTime> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Max);
            return DbSet.GetDateTimeValueAsync(Operations);
        }

        public Task<QueryResult<double>> MaxAsync(IPropertySelector<double> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Max);
            return DbSet.GetDoubleValueAsync(Operations);

        }
        public Task<QueryResult<int>> MinAsync(IPropertySelector<int> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Min);
            return DbSet.GetIntValueAsync(Operations);
        }

        public Task<QueryResult<double>> MinAsync(IPropertySelector<double> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Min);
            return DbSet.GetDoubleValueAsync(Operations);
        }

        public Task<QueryResult<DateTime>> MinAsync(IPropertySelector<DateTime> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Max);
            return DbSet.GetDateTimeValueAsync(Operations);
        }

        public Task<QueryResult<double>> AvgAsync(IPropertySelector<int> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Avg);
            return DbSet.GetDoubleValueAsync(Operations);
        }

        public Task<QueryResult<double>> AvgAsync(IPropertySelector<double> propertySelector)
        {
            AddPropertyOperation(propertySelector, QueryTypes.Avg);
            return DbSet.GetDoubleValueAsync(Operations);
        }

        #endregion

        public Task<QueryResult<List<T>>> ToListAsync()
        {
            return DbSet.GetEntitiesAsync(Operations);
        }


        void AddBoolOperation(IQueryExpression[] expressions, QueryTypes queryType)
        {
            Console.WriteLine("QueryAggregator AddBoolOperation expressions.Length: " + expressions.Length);

            Operations.Add(new Tuple<QueryTypes, IExpressionBuilder, ValueTypes>
                                (queryType, expressions.GroupExpressions(), ValueTypes.Bool));
        }
        
        void AddPropertyOperation<TValue>(IPropertySelector<TValue> propertySelector, QueryTypes queryType)
        {
            ValueTypes valueType;

            if (typeof(TValue) == typeof(string))
                valueType = ValueTypes.String;
            else if (typeof(TValue) == typeof(int))
                valueType = ValueTypes.Int;
            else if (typeof(TValue) == typeof(double))
                valueType = ValueTypes.Double;
            else if (typeof(TValue) == typeof(Guid))
                valueType = ValueTypes.Guid;
            else if (typeof(TValue) == typeof(bool))
                valueType = ValueTypes.Bool;
            else if (typeof(TValue) == typeof(DateTime))
                valueType = ValueTypes.DateTime;
            else if (typeof(TValue) == typeof(Byte[]))
                valueType = ValueTypes.ByteArray;
            else
                valueType = ValueTypes.String;

            Operations.Add(new Tuple<QueryTypes, IExpressionBuilder, ValueTypes>
                                (queryType, propertySelector, valueType));
        }
    }
}
