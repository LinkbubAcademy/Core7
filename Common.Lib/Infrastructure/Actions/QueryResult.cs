using Common.Lib.Core;

namespace Common.Lib.Infrastructure.Actions
{
    public class QueryResult : ActionResult
    {
        public Dictionary<Guid, Entity> ReferencedEntities { get; set; } = new Dictionary<Guid, Entity>();
    }

    public class QueryResult<T> : QueryResult
    {
        public T? Value { get; set; }

        public QueryResult()
        {

        }

        public QueryResult(T? value)
        {
            Value = value;
            IsSuccess = value is not null;
        }
    }
}
