using Common.Lib.Core;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryEntityResult
    {
        public QueryEntityResult(QueryResult<Entity> qr)
        {
            ActionResult = new ActionResult()
            {
                IsSuccess = qr.IsSuccess,
                Message = qr.Message
            };

            if (qr.IsSuccess && qr.Value != null)
                this.SValue = new EntityInfo(qr.Value.GetChanges());

            

            qr?.ReferencedEntities.DoForeach(e => SReferencedEntities.Add(e.Key.ToString(), new EntityInfo(e.Value.GetChanges())));
        }
    }
}
