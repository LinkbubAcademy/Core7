using Common.Lib.Infrastructure;

namespace Common.Lib.Services.Protobuf
{
    public partial class UnitOfWorkResult : IActionResult
    {
        public UnitOfWorkResult(IActionResult actionResult)
        {
            IsSuccess = actionResult.IsSuccess;
            Message = actionResult.Message;
            Errors = actionResult.Errors;
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
