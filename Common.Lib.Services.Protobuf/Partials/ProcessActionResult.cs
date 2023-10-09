using Common.Lib.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Lib.Services.Protobuf
{
    public partial class ProcessActionResult : IProcessActionResult
    {
        public ProcessActionResult(IProcessActionResult result)
        {
            this.Errors = result.Errors;
            this.IsSuccess = result.IsSuccess;
            this.Message = result.Message;
            this.OutputType = result.OutputType;
            this.IsCollection = result.IsCollection;
            this.Serialized = result.Serialized;
            this.OutputClassName = result.OutputClassName;            
        }

        public IProcessActionResult.OutputTypes OutputType
        {
            get
            {
                return (IProcessActionResult.OutputTypes)NOutputType;
            }
            set
            {
                NOutputType = (int)value;
            }
        }

        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public void AddErrors(IEnumerable<string> errors)
        {
            ((List<string>)Errors).AddRange(errors);
        }
    }
}
