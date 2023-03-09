using System.Linq.Expressions;
using Common.Lib.Core;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Authentication
{
    public class UserEmail : IPropertySelector<string>
    {
        public ValueTypes PropertyType => ValueTypes.String;

        public Expression<Func<TEntity, string>>? CreateSelector<TEntity>() where TEntity : Entity
        {
            if (typeof(User).IsAssignableFrom(typeof(TEntity)))
            {
                return (x) => (x as User).Email;
            }
            return default;
        }

        public IPropertySelector CreateSelector()
        {
            return new UserEmail();
        }

        public static UserEmail Property { get => new UserEmail(); }

        #region Equals

        public static UserByEmail EqualsTo(string email,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return UserByEmail.Create(email, ComparisonTypes.Equals, opType);
        }

        public static UserByEmail AndEqualsTo(string email)
        {
            return UserByEmail.Create(email, ComparisonTypes.Equals, LogicalOperationTypes.And);
        }

        public static UserByEmail OrEqualsTo(string email)
        {
            return UserByEmail.Create(email, ComparisonTypes.Equals, LogicalOperationTypes.Or);
        }

        #endregion

        #region NotEquals

        public static UserByEmail NotEqualsTo(string email,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return UserByEmail.Create(email, ComparisonTypes.NotEqual, opType);
        }

        public static UserByEmail AndNotEqualsTo(string email)
        {
            return UserByEmail.Create(email, ComparisonTypes.NotEqual, LogicalOperationTypes.And);
        }

        public static UserByEmail OrNotEqualsTo(string email)
        {
            return UserByEmail.Create(email, ComparisonTypes.NotEqual, LogicalOperationTypes.Or);
        }

        #endregion

    }
}
