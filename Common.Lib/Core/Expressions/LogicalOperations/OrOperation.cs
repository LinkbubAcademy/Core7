using Common.Lib.Core.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class OrOperation : IQueryExpression
    {
        public LogicalOperationTypes LogicalOperationType { get; set; } = LogicalOperationTypes.Or;
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