using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Infrastructure
{
    public interface IProcessActionResult : IActionResult
    {
        string Serialized { get; set; }

        public OutputTypes OutputType { get; set; }

        public bool IsCollection { get; set; }

        public string OutputClassName { get; set; }

        public enum OutputTypes
        {
            Void,
            SimpleType,
            Dto,
            Model
        }

        public IActionResult ToQueryResultSimpleType()
        {
            IActionResult output = null;
            switch (OutputClassName)
            {
                case "Guid":
                    output = new QueryResult<Guid>() { Value = Guid.Parse(Serialized) };
                    break;
                default:
                    return new QueryResult();

            }

            output.Message = Message;   
            output.IsSuccess = IsSuccess;
            output.AddErrors(Errors);

            return output;
        }
    }
}
