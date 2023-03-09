using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public interface IQueryExpression : IExpressionBuilder
    {
        LogicalOperationTypes LogicalOperationType { get; set; }
        ComparisonTypes ComparisonType { get; set; }

        string[] GetParameters();

        IQueryExpression Create(string[] parameters);
    }

    public interface IQueryExpression<TValue> : IQueryExpression
    {
        Expression<Func<TEntity, TValue>>? CreateExpression<TEntity>() where TEntity : Entity;

    }
}
