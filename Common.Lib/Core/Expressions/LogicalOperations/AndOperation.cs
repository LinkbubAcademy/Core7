using Common.Lib.Core.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class AndOperation : IQueryExpression
    {
        public LogicalOperationTypes LogicalOperationType { get; set; } = LogicalOperationTypes.And;
        public ComparisonTypes ComparisonType { get; set; }

        public IQueryExpression Create(string[] parameters)
        {
            return this;
        }

        public string[] GetParameters()
        {
            return new string[0];
        }
    }
}