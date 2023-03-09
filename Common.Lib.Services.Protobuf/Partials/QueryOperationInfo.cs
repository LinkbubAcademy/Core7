using Common.Lib.Core.Expressions;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryOperationInfo : IQueryOperationInfo
    {
        public QueryOperationInfo(int queryType, object expressions)
        {
            QueryType = queryType;

            if (expressions is IQueryExpression[])
                ((IQueryExpression[])expressions)
                    .DoForeach(exp => SExpressions.Add(new QueryExpressionInfo(exp)));            
            else if(expressions is IPropertySelector[])
                SExpressions.Add(new QueryExpressionInfo(((IPropertySelector[])expressions)[0]));
            else if (expressions is IPropertySelector)
                SExpressions.Add(new QueryExpressionInfo((IPropertySelector)expressions));
            else
            {
                Console.WriteLine($"QueryOperationInfo.ctor expressions type: {expressions.GetType().FullName}");
            }
        }

        public IEnumerable<IQueryExpressionInfo> Expressions
        {
            get
            {
                return this.SExpressions;
            }
        }
    }
}
