namespace Common.Lib.Core.Expressions
{
    public class ExpressionsGroup : IExpressionBuilder
    {
        public IQueryExpression[] Expressions { get; set; }

        public ExpressionsGroup(IQueryExpression[] expressions)
        {
            Expressions = expressions;
        }
    }
}
