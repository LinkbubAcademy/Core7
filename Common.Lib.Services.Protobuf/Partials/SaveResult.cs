using Common.Lib.Infrastructure;

namespace Common.Lib.Services.Protobuf
{
    public partial class SaveResult : ISaveResult
    {
        public SaveResult(ISaveResult saveResult)
        {
            IsSuccess = saveResult.IsSuccess;
            Message = saveResult.Message;
            Errors = saveResult.Errors;
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
    }
}
