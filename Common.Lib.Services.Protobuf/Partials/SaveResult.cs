using Common.Lib.Core;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Services.Protobuf
{
    public partial class SaveResult : ISaveResult
    {
        public bool IsMaintenanceModeOn { get; set; }
        public SaveResult(ISaveResult saveResult)
        {
            IsSuccess = saveResult.IsSuccess;
            Message = saveResult.Message;
            Errors = saveResult.Errors;

            var qr = saveResult.AsQueryResult();

            if (qr.IsSuccess && qr.Value != null)
                this.SValue = new EntityInfo(qr.Value.GetChanges());

            qr?.ReferencedEntities.DoForeach(e => SReferencedEntities.Add(e.Key.ToString(), new EntityInfo(e.Value.GetChanges())));
        }

        public SaveResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = new List<string>();
        }

        public IEnumerable<string> Errors
        {
            get
            {
                return sErrors_;
            }
            set
            {
                foreach (var error in value)
                    sErrors_.Add(error);
            }
        }

        public QueryResult<Entity> AsQueryResult()
        {
            throw new NotImplementedException();
        }
        
    }
}
