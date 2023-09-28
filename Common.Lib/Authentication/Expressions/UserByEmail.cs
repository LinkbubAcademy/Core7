using System.Linq.Expressions;
using Common.Lib.Core.Expressions;

namespace Common.Lib.Authentication
{
    public class UserByEmail : BaseExpression<bool>
    {
        public string Email { get; set; } = string.Empty;

        public override Expression<Func<TEntity, bool>>? CreateExpression<TEntity>()
        {            
            if (typeof(User).IsAssignableFrom(typeof(TEntity)))
            {
                switch (ComparisonType)
                {
                    case ComparisonTypes.Equals:
                        return (x) => (x as User).Email == Email;
                    case ComparisonTypes.NotEqual:
                        return (x) => (x as User).Email != Email;
                    case ComparisonTypes.Contains: 
                        return (x) => (x as User).Email.Contains(Email);
                    case ComparisonTypes.NotContains:
                        return (x) => !(x as User).Email.Contains(Email);

                    default:
                        throw new ArgumentException($"OperationType:{ComparisonType.ToString()} is not valid for a string property");
                }
            }
            return default;
        }

        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { Email } :
                            new string[2] { Email, ((int)ComparisonType).ToString() };
        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new UserByEmail()
            {
                Email = parameters[0]
            };
        }

        internal static UserByEmail Create(string email,
                                                ComparisonTypes comparisonType = ComparisonTypes.Equals,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return new UserByEmail() { Email = email, ComparisonType = comparisonType, LogicalOperationType = opType };
        }
    }
}
