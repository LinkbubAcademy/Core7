using System.Linq.Expressions;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Authentication
{
    public class UserByAccessLevel : BaseExpression<bool>
    {
        public int AccessLevel { get; set; }

        public override Expression<Func<TEntity, bool>>? CreateExpression<TEntity>()
        {
            if (typeof(User).IsAssignableFrom(typeof(TEntity)))
            {
                switch (ComparisonType)
                {
                    default:
                    case ComparisonTypes.Equals:
                        return (x) => (x as User).AccessLevel == AccessLevel;
                    case ComparisonTypes.NotEqual:
                        return (x) => (x as User).AccessLevel != AccessLevel;
                    case ComparisonTypes.Upper:
                        return (x) => (x as User).AccessLevel > AccessLevel;
                    case ComparisonTypes.UpperOrEqual:
                        return (x) => (x as User).AccessLevel >= AccessLevel;
                    case ComparisonTypes.Lower:
                        return (x) => (x as User).AccessLevel < AccessLevel;
                    case ComparisonTypes.LowerOrEqual:
                        return (x) => (x as User).AccessLevel <= AccessLevel;                    
                }
            }
            return default;
        }

        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { AccessLevel.ToString() } :
                            new string[2] { AccessLevel.ToString(), ((int)ComparisonType).ToString() };

        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new UserByAccessLevel()
            {
                AccessLevel = int.Parse(parameters[0])
            };
        }

        internal static UserByAccessLevel Create(int accessLevel,
                                                ComparisonTypes comparisonType = ComparisonTypes.Equals,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return new UserByAccessLevel() { AccessLevel = accessLevel, ComparisonType = comparisonType, LogicalOperationType = opType };
        }



    }
}
