using System.Linq.Expressions;
using Common.Lib.Authentication;
using Common.Lib.Core;
using Common.Lib.Core.Expressions;

namespace Test.Lib.Models
{
    public class PostOwner : IPropertySelector<Guid>
    {
        public ValueTypes PropertyType => ValueTypes.Guid;

        public Expression<Func<TEntity, Guid>>? CreateSelector<TEntity>() where TEntity : Entity
        {
            if (typeof(Post).IsAssignableFrom(typeof(TEntity)))
            {
                return (x) => (x as Post).OwnerId;
            }
            return default;
        }

        public IPropertySelector CreateSelector()
        {
            return new PostOwner();
        }

        public static PostOwner Property { get => new PostOwner(); }

        #region Equals

        public static PostByOwner EqualsTo(Guid ownerId,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return PostByOwner.Create(ownerId, ComparisonTypes.Equals, opType);
        }

        public static PostByOwner AndEqualsTo(Guid ownerId)
        {
            return PostByOwner.Create(ownerId, ComparisonTypes.Equals, LogicalOperationTypes.And);
        }

        public static PostByOwner OrEqualsTo(Guid ownerId)
        {
            return PostByOwner.Create(ownerId, ComparisonTypes.Equals, LogicalOperationTypes.Or);
        }

        #endregion

        #region NotEquals

        public static PostByOwner NotEqualsTo(Guid ownerId,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return PostByOwner.Create(ownerId, ComparisonTypes.NotEqual, opType);
        }

        public static PostByOwner AndNotEqualsTo(Guid ownerId)
        {
            return PostByOwner.Create(ownerId, ComparisonTypes.NotEqual, LogicalOperationTypes.And);
        }

        public static PostByOwner OrNotEqualsTo(Guid ownerId)
        {
            return PostByOwner.Create(ownerId, ComparisonTypes.NotEqual, LogicalOperationTypes.Or);
        }

        #endregion

    }
}
