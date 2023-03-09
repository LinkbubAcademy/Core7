using Common.Lib.Core;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryEntitiesResult
    {
        public QueryEntitiesResult(QueryResult<List<Entity>> qr)
        {
            ActionResult = new ActionResult()
            {
                IsSuccess = true,
                Message = qr.Message
            };

            qr?.Value?.DoForeach(e => this.SValue.Add(new EntityInfo(e.GetChanges())));
            qr?.ReferencedEntities.DoForeach(e => SReferencedEntities.Add(e.Key.ToString(), new EntityInfo(e.Value.GetChanges())));
        }
    }
}
