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
            // todo: completar
            IActionResult output = null;
            switch (OutputClassName)
            {
                case "Guid":
                    output = new QueryResult<Guid>() { IsSuccess = this.IsSuccess, Value = Guid.Parse(Serialized) };
                    break;
                case "Bool":
                    output = new QueryResult<bool>() { IsSuccess = this.IsSuccess, Value = Serialized == "True" };
                    break;
                default:
                    Log.WriteLine(OutputClassName);
                    return new QueryResult();

            }

            output.Message = Message;
            output.IsSuccess = IsSuccess;
            output.AddErrors(Errors);

            return output;
        }
    }
}
