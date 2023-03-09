using System.Linq.Expressions;
using Common.Lib.Core.Expressions;
using Test.Lib.Models;

namespace Common.Lib.Authentication
{
    public class PostByOwner : BaseExpression<bool>
    {
        public Guid OwnerId { get; set; }

        public override Expression<Func<TEntity, bool>>? CreateExpression<TEntity>()
        {
            if (typeof(Post).IsAssignableFrom(typeof(TEntity)))
            {
                switch (ComparisonType)
                {
                    case ComparisonTypes.Equals:
                        return (x) => (x as Post).OwnerId == OwnerId;
                    case ComparisonTypes.NotEqual:
                        return (x) => (x as Post).OwnerId != OwnerId;
                    default:
                        throw new ArgumentException($"OperationType:{ComparisonType.ToString()} is not valid for a guid property");
                }
            }
            return default;
        }

        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { OwnerId.ToString() } :
                            new string[2] { OwnerId.ToString(), ((int)ComparisonType).ToString() };
        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new PostByOwner()
            {
                OwnerId = Guid.Parse(parameters[0])
            };
        }

        internal static PostByOwner Create(Guid ownerId,
                                                ComparisonTypes comparisonType = ComparisonTypes.Equals,
                                                LogicalOperationTypes opType = LogicalOperationTypes.And)
        {
            return new PostByOwner() { OwnerId = ownerId, ComparisonType = comparisonType, LogicalOperationType = opType };
        }
    }
}
