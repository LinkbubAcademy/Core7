using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public interface IPropertySelector : IExpressionBuilder
    {
        ValueTypes PropertyType { get; }
        IPropertySelector CreateSelector();
        
    }

    public interface IPropertySelector<TValue> : IPropertySelector
    {
        Expression<Func<TEntity, TValue>>? CreateSelector<TEntity>() where TEntity : Entity;
    }
}
