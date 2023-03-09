using Common.Lib.Core.Expressions;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryExpressionInfo : IQueryExpressionInfo
    {
        public QueryExpressionInfo(IQueryExpression exp)
        {
            this.SComparisonType = (int)exp.ComparisonType;            
            this.SLogicalOperationType = (int)exp.LogicalOperationType;

            this.Type = exp.GetType().FullName;
            exp.GetParameters().DoForeach(p => this.SParameters.Add(p));

        }

        public QueryExpressionInfo(IPropertySelector ps)
        {
            this.IsSelector = true;
            this.Type = ps.GetType().FullName;
        }

        public IEnumerable<string> SerializedParameters
        {
            get
            {
                return this.SParameters;
            }
        }

        public LogicalOperationTypes LogicalOperationType { get => (LogicalOperationTypes)SLogicalOperationType; }
        public ComparisonTypes ComparisonType { get => (ComparisonTypes)SComparisonType; }
    }
}
