namespace Common.Lib.Core.Expressions
{
    public interface IQueryExpressionInfo
    {
        LogicalOperationTypes LogicalOperationType { get; }
        ComparisonTypes ComparisonType { get; }
        bool IsSelector { get; set; }
        string Type { get; set; }

        IEnumerable<string> SerializedParameters { get; }
    }
}
