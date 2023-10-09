using Common.Lib.Core;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Infrastructure
{
    public interface ISaveResult : IActionResult
    {
        QueryResult<Entity> AsQueryResult();

    }

    public interface ISaveResult<T> : ISaveResult where T : Entity, new()
    {
        T? Value { get; set; }
        ISaveResult<TOut> CastoTo<TOut>() where TOut : Entity, new();
    }

}
