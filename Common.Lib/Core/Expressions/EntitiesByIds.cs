using Common.Lib.Authentication;
using System.Linq.Expressions;

namespace Common.Lib.Core.Expressions
{
    public class EntitiesByIds : BaseExpression<bool>
    {
        public HashSet<Guid> Ids { get; set; }

        public override Expression<Func<TEntity, bool>> CreateExpression<TEntity>()
        {
            switch (ComparisonType)
            {
                case ComparisonTypes.Equals:
                    return (x) => Ids.Contains(x.Id);
                case ComparisonTypes.NotEqual:
                    return (x) => !Ids.Any(id => x.Id == id);
                default:
                    throw new ArgumentException($"OperationType:{ComparisonType.ToString()} is not valid for a Guid property");
            }
        }
        public override string[] GetParameters()
        {
            return ComparisonType == ComparisonTypes.Equals ?
                            new string[1] { string.Join('¬',Ids) } :
                            new string[2] { string.Join('¬', Ids), ((int)ComparisonType).ToString() };
        }

        public override IQueryExpression Create(string[] parameters)
        {
            var spaso = parameters[0].Split('¬');
            return new EntitiesByIds()
            {
                Ids = new HashSet<Guid>(spaso.Select(s => Guid.Parse(s)))
            };
        }

        public static EntitiesByIds Create(IEnumerable<Guid> ids, ComparisonTypes operationType = ComparisonTypes.Equals)
        {
            return new EntitiesByIds() { Ids = new HashSet<Guid>(ids), ComparisonType = operationType };
        }
    }
}
