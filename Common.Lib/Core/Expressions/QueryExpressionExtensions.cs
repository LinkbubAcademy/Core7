using Common.Lib.Core.Metadata;
using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public static class QueryExpressionExtensions
    {
        static Type AndOperationType { get; set; } = typeof(AndOperation);
        static Type OrOperationType { get; set; } = typeof(OrOperation);

        public static ExpressionsGroup GroupExpressions(this IQueryExpression[] expressions)
        {
            return new ExpressionsGroup(expressions);           
        }

        public static IQueryExpression<bool> CombineExpressions<TEntity>(this IQueryExpression[] expressions) where TEntity : Entity
        {
            if (expressions == null || expressions.Length == 0)
                return new CombinedExpressionsTransporter<TEntity>();

            var output = ((IQueryExpression<bool>)expressions[0]).CreateExpression<TEntity>();

            if (expressions.Length == 1)
                return new CombinedExpressionsTransporter<TEntity>();

            for (var i = 1; i < expressions.Length; i++)
            {
                var qExp = ((IQueryExpression<bool>)expressions[i]);
                var expToAdd = qExp.CreateExpression<TEntity>(); ;

                output = qExp.LogicalOperationType == LogicalOperationTypes.And ?
                                        And(output, expToAdd) :
                                        Or(output, expToAdd);
            }

            return new CombinedExpressionsTransporter<TEntity>(output);
        }

        static Expression<Func<TEntity, bool>> And<TEntity>(Expression<Func<TEntity, bool>> expr1,
                                                       Expression<Func<TEntity, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<TEntity, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        static Expression<Func<TEntity, bool>> Or<TEntity>(Expression<Func<TEntity, bool>> expr1,
                                                       Expression<Func<TEntity, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<TEntity, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static IQueryExpression[] ToQueryEpxressions(this IEnumerable<IQueryExpressionInfo> queryInfos)
        {
            var output = new List<IQueryExpression>();

            var nextOpType = AndOperationType;

            foreach (var info in queryInfos)
            {
                if (MetadataHandler.QueryExpressionConstructors.TryGetValue(info.Type, out Func<string[], IQueryExpression> value))
                {
                    var ctor = value;
                    var qExp = ctor(info.SerializedParameters.ToArray());

                    if (qExp is OrOperation)
                    {
                        nextOpType = OrOperationType;
                        continue;
                    }

                    if (qExp is AndOperation)
                    {
                        nextOpType = AndOperationType;
                        continue;
                    }

                    qExp.ComparisonType = info.ComparisonType;
                    qExp.LogicalOperationType = nextOpType == AndOperationType ? 
                                                    LogicalOperationTypes.And : LogicalOperationTypes.Or;
                    output.Add(qExp);
                    nextOpType = AndOperationType;
                }
                else
                {
                    throw new Exception($"Expression {info.Type} not registered");
                }
            }

            return output.ToArray();
        }

        public static IPropertySelector ToPropertySelector(this IQueryExpressionInfo queryInfo)
        {
            var output = MetadataHandler.PropertySelectorsConstructors[queryInfo.Type]();

            return output;
        }
    }
}
