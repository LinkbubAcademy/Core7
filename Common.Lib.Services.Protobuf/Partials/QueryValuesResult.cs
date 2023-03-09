using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryValuesResult
    {
        public QueryValuesResult(QueryResult<List<object>> qr)
        {
            ActionResult = new ActionResult()
            {
                IsSuccess = true,
                Message = qr.Message
            };

            if(qr.IsSuccess && qr.Value != null && qr.Value.Count > 0)
            {
                var oType = qr.Value[0].GetType();

                var serializeFunc = EntityInfo.GetSerializeFunc(oType);
                qr.Value.DoForeach(o => this.SValue.Add(serializeFunc(o)));
            }
        }
    }
}
