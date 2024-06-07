using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class EntityById : BaseExpression<bool>
    {
        public Guid Id { get; set; }

        public override Expression<Func<TEntity, bool>> CreateExpression<TEntity>()
        {
            switch (ComparisonType)
            {
                case ComparisonTypes.Equals:
                    return (x) => x.Id == Id;
                case ComparisonTypes.NotEqual:
                    return (x) => x.Id != Id;
                default:
                    throw new ArgumentException($"OperationType:{ComparisonType.ToString()} is not valid for a Guid property");
            }
        }
        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { Id.ToString() } :
                            new string[2] { Id.ToString(), ((int)ComparisonType).ToString() };
        }

        public override IQueryExpression Create(string[] parameters)
        {
            return new EntityById()
            {
                Id = Guid.Parse(parameters[0])
            };
        }

        public static EntityById Create(Guid id, ComparisonTypes operationType = ComparisonTypes.Equals)
        {
            return new EntityById() { Id = id, ComparisonType = operationType };
        }
    }
}
