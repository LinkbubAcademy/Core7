using System.Linq.Expressions;
using Common.Lib.Core;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class EntityByCreatedOn : BaseExpression<bool>
    {
        public DateTime CreatedOn { get; set; }

        public override Expression<Func<TEntity, bool>>? CreateExpression<TEntity>()
        {
            if (typeof(Entity).IsAssignableFrom(typeof(TEntity)) || typeof(TEntity) == typeof(Entity))
            {
                switch (ComparisonType)
                {
                    default:
                    case ComparisonTypes.Equals:
                        return (x) => (x as Entity).CreatedOn == CreatedOn;
                    case ComparisonTypes.NotEqual:
                        return (x) => (x as Entity).CreatedOn != CreatedOn;
                    case ComparisonTypes.Upper:
                        return (x) => (x as Entity).CreatedOn > CreatedOn;
                    case ComparisonTypes.UpperOrEqual:
                        return (x) => GreaterOrEquals((x as Entity), (x as Entity).CreatedOn, CreatedOn);
                    case ComparisonTypes.Lower:
                        return (x) => (x as Entity).CreatedOn < CreatedOn;
                    case ComparisonTypes.LowerOrEqual:
                        return (x) => LowerOrEquals((x as Entity), (x as Entity).CreatedOn, CreatedOn);                    
                }
            }
            return default;
        }

        public bool GreaterOrEquals(Entity ent, DateTime dt1, DateTime dt2)
        {
            return dt1 >= dt2;
        }

        public bool LowerOrEquals(Entity ent, DateTime dt1, DateTime dt2)
        {
            return dt1 <= dt2;
        }

        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { CreatedOn.Ticks.ToString() } :
                            new string[2] { CreatedOn.Ticks.ToString(), ((int)ComparisonType).ToString() };

        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new EntityByCreatedOn()
            {
                CreatedOn = new DateTime().AddTicks(long.Parse(parameters[0].ToString()))
            };
        }

        internal static EntityByCreatedOn Create(DateTime createdOn,
                                                ComparisonTypes comparisonType = ComparisonTypes.Equals,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return new EntityByCreatedOn() { CreatedOn = createdOn, ComparisonType = comparisonType, LogicalOperationType = opType };
        }
    }
}
