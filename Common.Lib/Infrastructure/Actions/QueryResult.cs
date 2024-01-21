using Common.Lib.Core;
using Common.Lib.Services;

namespace Common.Lib.Infrastructure.Actions
{
    public class QueryResult : ActionResult
    {
        public Dictionary<Guid, Entity> ReferencedEntities { get; set; } = new Dictionary<Guid, Entity>();

        public virtual IProcessActionResult ToProcessActionResult(
                                                            IProcessActionResult.OutputTypes outputType,
                                                            bool isCollection,
                                                            string className)
        {
            var output = new ProcessActionResult()
            {
                Serialized = string.Empty,
                OutputType = outputType,
                IsCollection = isCollection,
                OutputClassName = className
            };

            return output;
        }
    }

    public class QueryResult<T> : QueryResult
    {
        public T? Value { get; set; }

        public QueryResult()
        {

        }

        public QueryResult(T? value)
        {
            Value = value;
            IsSuccess = value is not null;
        }

        public override IProcessActionResult ToProcessActionResult(
                                                            IProcessActionResult.OutputTypes outputType,
                                                            bool isCollection,
                                                            string className)
        {
            var output = new ProcessActionResult()
            {
                IsSuccess = IsSuccess,
                Serialized =  string.Empty,
                OutputType = outputType,
                IsCollection = isCollection,
                OutputClassName = className
            };

            if (Value != null)
            {
                switch (outputType)
                {
                    //Todo: complete all possibilities
                    case IProcessActionResult.OutputTypes.SimpleType:
                        if (className == "Guid")
                        {
                            output.Serialized = Value.ToString();
                        }
                        else if(className =="Bool")
                        {
                            output.Serialized = Value.ToString();
                        }
                        break;
                    case IProcessActionResult.OutputTypes.Dto:
                        output.Serialized = isCollection ? Dto.SerializeCollection((IEnumerable<object>)Value) : (Value as Dto).Serialize();
                        4break;
                    case IProcessActionResult.OutputTypes.Model:
                        break;
                    case IProcessActionResult.OutputTypes.Void:
                    default:
                            break;
                }
            }

            return output;
        }
    }
}
