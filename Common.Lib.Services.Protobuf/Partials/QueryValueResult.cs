using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Services.Protobuf
{
    public partial class QueryValueResult
    {
        public void AssignResult<TValue>(QueryResult<TValue> qr)
        {
            ActionResult = new ActionResult()
            {
                IsSuccess = true,
                Message = qr.Message
            };

            if (qr.IsSuccess && qr.Value != null)
            {
                var oType = qr.Value.GetType();

                var serializeFunc = EntityInfo.GetSerializeFunc(oType);
                this.SValue = serializeFunc(qr.Value);
            }
        }
    }
}
