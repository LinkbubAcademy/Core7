using Common.Lib.Core;
using Common.Lib.Infrastructure;

namespace Common.Lib.Services.Protobuf
{
    public partial class DeleteResult : IDeleteResult
    {
        public DeleteResult(IDeleteResult deleteResult)
        {
            IsSuccess = deleteResult.IsSuccess;
            Message = deleteResult.Message;
            Errors = deleteResult.Errors;
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

        public Entity? DeletedEntity { get; set; }

        public void AddErrors(IEnumerable<string> errors)
        {
            ((List<string>)Errors).AddRange(errors);
        }
    }
}
