namespace Common.Lib.Core.Expressions
{
    public interface IQueryOperationInfo
    {
        int QueryType { get; set; }

        IEnumerable<IQueryExpressionInfo> Expressions { get; }
    }
}
