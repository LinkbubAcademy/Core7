using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Lib.Core.Expressions
{
    public class CombinedExpressionsTransporter<TEntity> : IQueryExpression<bool>
    {
        public LogicalOperationTypes LogicalOperationType { get; set; }
        public ComparisonTypes ComparisonType { get; set; }

        public Expression<Func<TEntity, bool>> CombinedExpressions { get; set; } = (x) => true;

        public CombinedExpressionsTransporter()
        {
        }

        public CombinedExpressionsTransporter(Expression<Func<TEntity, bool>> combinedExpression)
        {
            CombinedExpressions = combinedExpression;
        }

        public IQueryExpression Create(string[] parameters)
        {
            return this;
        }

        public Expression<Func<TEntity1, bool>>? CreateExpression<TEntity1>() where TEntity1 : Entity
        {
            return CombinedExpressions as Expression<Func<TEntity1, bool>>;
        }

        public string[] GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
