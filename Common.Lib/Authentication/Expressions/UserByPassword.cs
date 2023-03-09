using System.Linq.Expressions;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Authentication
{
    public class UserByPassword : BaseExpression<bool>
    {
        public string Password { get; set; } = string.Empty;

        public override Expression<Func<TEntity, bool>>? CreateExpression<TEntity>()
        {
            if (typeof(User).IsAssignableFrom(typeof(TEntity)))
            {
                switch (ComparisonType)
                {
                    case ComparisonTypes.Equals:
                        return (x) => (x as User).Password == Password;
                    case ComparisonTypes.NotEqual:
                        return (x) => (x as User).Password != Password;
                    default:
                        throw new ArgumentException($"OperationType:{ComparisonType.ToString()} is not valid for a string property");
                }
            }
            return default;
        }

        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { Password } :
                            new string[2] { Password, ((int)ComparisonType).ToString() };
        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new UserByPassword()
            {
                Password = parameters[0]
            };
        }

        public static UserByPassword Create(string password, 
                                                ComparisonTypes comparisonType = ComparisonTypes.Equals,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return new UserByPassword() { Password = password, ComparisonType = comparisonType, LogicalOperationType = opType };
        }

        public static UserByPassword And(string password, ComparisonTypes comparisonType = ComparisonTypes.Equals)
        {
            return Create(password, comparisonType, LogicalOperationTypes.And);
        }
        public static UserByPassword Or(string password, ComparisonTypes comparisonType = ComparisonTypes.Equals)
        {
            return Create(password, comparisonType, LogicalOperationTypes.Or);
        }
    }
}
