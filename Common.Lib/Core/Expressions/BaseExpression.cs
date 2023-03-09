using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public abstract class BaseExpression<TValue> : IQueryExpression<TValue>
    {
        public LogicalOperationTypes LogicalOperationType { get; set; } = LogicalOperationTypes.And;
        public ComparisonTypes ComparisonType { get; set; } = ComparisonTypes.Equals;

        public virtual string[] GetParameters()
        {
            throw new NotImplementedException();
        }

        public virtual Expression<Func<TEntity, TValue>>? CreateExpression<TEntity>() where TEntity : Entity
        {
            throw new NotImplementedException();
        }

        public virtual IQueryExpression Create(string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
