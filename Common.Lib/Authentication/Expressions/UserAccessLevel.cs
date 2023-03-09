using System.Linq.Expressions;
using System.Net.Http.Headers;
using Common.Lib.Core;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Authentication
{
    public class UserAccessLevel : IPropertySelector<int>
    {
        public ValueTypes PropertyType => ValueTypes.Int;

        public Expression<Func<TEntity, int>>? CreateSelector<TEntity>() where TEntity : Entity
        {
            if (typeof(User).IsAssignableFrom(typeof(TEntity)))
            {
                return (x) => (x as User).AccessLevel;
            }
            return default;
        }

        public IPropertySelector CreateSelector()
        { 
            return new UserAccessLevel();
        }

        public static UserAccessLevel Property { get => new UserAccessLevel(); }

        #region Equals

        public static UserByAccessLevel EqualsTo(int accessLevel,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.Equals, opType);
        }

        public static UserByAccessLevel AndEqualsTo(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.Equals, LogicalOperationTypes.And);
        }

        public static UserByAccessLevel OrEqualsTo(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.Equals, LogicalOperationTypes.Or);
        }

        #endregion

        #region NotEquals

        public static UserByAccessLevel NotEqualsTo(int accessLevel,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.NotEqual, opType);
        }

        public static UserByAccessLevel AndNotEqualsTo(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.NotEqual, LogicalOperationTypes.And);
        }

        public static UserByAccessLevel OrNotEqualsTo(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.NotEqual, LogicalOperationTypes.Or);
        }

        #endregion

        #region Upper

        public static UserByAccessLevel GreaterThan(int accessLevel,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.Upper, opType);
        }

        public static UserByAccessLevel AndGreaterThan(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.Upper, LogicalOperationTypes.And);
        }

        public static UserByAccessLevel OrGreaterThan(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.Upper, LogicalOperationTypes.Or);
        }

        #endregion

        #region UpperOrEqual

        public static UserByAccessLevel GreaterOrEqualTo(int accessLevel,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.UpperOrEqual, opType);
        }

        public static UserByAccessLevel AndGreaterOrEqualTo(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.UpperOrEqual, LogicalOperationTypes.And);
        }

        public static UserByAccessLevel OrGreaterOrEqualTo(int accessLevel)
        {
            return UserByAccessLevel.Create(accessLevel, ComparisonTypes.UpperOrEqual, LogicalOperationTypes.Or);
        }

        #endregion

    }
}
