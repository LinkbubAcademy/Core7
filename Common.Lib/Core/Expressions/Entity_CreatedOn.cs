using System.Linq.Expressions;
using Common.Lib.Core;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class Entity_CreatedOn : IPropertySelector<DateTime> 
    {
        public ValueTypes PropertyType => ValueTypes.DateTime;

        public Expression<Func<TEntity, DateTime>>? CreateSelector<TEntity>() where TEntity : Entity
        {
              return (x) => (x as Entity).CreatedOn;
        }

        public IPropertySelector CreateSelector()
        {
            return new Entity_CreatedOn();
        }

        public static Entity_CreatedOn Property { get => new Entity_CreatedOn(); }

        #region Equals

        public static EntityByCreatedOn EqualsTo(DateTime createdOn,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Equals, opType);
        }

        public static EntityByCreatedOn AndEqualsTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Equals, LogicalOperationTypes.And);
        }

        public static EntityByCreatedOn OrEqualsTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Equals, LogicalOperationTypes.Or);
        }

        #endregion

        #region NotEquals

        public static EntityByCreatedOn NotEqualsTo(DateTime createdOn,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.NotEqual, opType);
        }

        public static EntityByCreatedOn AndNotEqualsTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.NotEqual, LogicalOperationTypes.And);
        }

        public static EntityByCreatedOn OrNotEqualsTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.NotEqual, LogicalOperationTypes.Or);
        }

        #endregion

        #region Upper

        public static EntityByCreatedOn GreaterThan(DateTime createdOn,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Upper, opType);
        }

        public static EntityByCreatedOn AndGreaterThan(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Upper, LogicalOperationTypes.And);
        }

        public static EntityByCreatedOn OrGreaterThan(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Upper, LogicalOperationTypes.Or);
        }

        #endregion

        #region UpperOrEqual

        public static EntityByCreatedOn GreaterOrEqualTo(DateTime createdOn,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.UpperOrEqual, opType);
        }

        public static EntityByCreatedOn AndGreaterOrEqualTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.UpperOrEqual, LogicalOperationTypes.And);
        }

        public static EntityByCreatedOn OrGreaterOrEqualTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.UpperOrEqual, LogicalOperationTypes.Or);
        }

        #endregion

        #region Lower

        public static EntityByCreatedOn LowerThan(DateTime createdOn,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Lower, opType);
        }

        public static EntityByCreatedOn AndLowerThan(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Lower, LogicalOperationTypes.And);
        }

        public static EntityByCreatedOn OrLowerThan(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.Lower, LogicalOperationTypes.Or);
        }

        #endregion

        #region LowerOrEqual

        public static EntityByCreatedOn LowerOrEqualTo(DateTime createdOn,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.LowerOrEqual, opType);
        }

        public static EntityByCreatedOn AndLowerOrEqualTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.LowerOrEqual, LogicalOperationTypes.And);
        }

        public static EntityByCreatedOn OrLowerOrEqualTo(DateTime createdOn)
        {
            return EntityByCreatedOn.Create(createdOn, ComparisonTypes.LowerOrEqual, LogicalOperationTypes.Or);
        }

        #endregion
    }
}
