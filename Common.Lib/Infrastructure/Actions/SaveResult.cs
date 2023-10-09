using Common.Lib.Core;

namespace Common.Lib.Infrastructure.Actions
{
    public class SaveResult<T> : ActionResult, ISaveResult<T> where T: Entity, new()
    {
        public T? Value { get; set; }

        public QueryResult<Entity> AsQueryResult()
        {
            return new QueryResult<Entity>()
            {
                IsSuccess = this.IsSuccess,
                Value = Value,
                ReferencedEntities = new()
            };
        }

        public ISaveResult<TOut> CastoTo<TOut>() where TOut : Entity, new()
        {
            var output = new SaveResult<TOut> { IsSuccess = this.IsSuccess, Value = Value as TOut };
            output.AddErrors(this.Errors);
            return output;
        }
    }
}
