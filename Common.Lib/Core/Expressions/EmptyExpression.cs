using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class EmptyExpression : BaseExpression<bool>
    {
        public override Expression<Func<TEntity, bool>> CreateExpression<TEntity>()
        {
            return (x) => true;
        }
        public override string[] GetParameters()
        {
            return Array.Empty<string>();
        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new EmptyExpression();
        }

        public static EmptyExpression Create()
        {
            return new EmptyExpression();
        }
    }
}
