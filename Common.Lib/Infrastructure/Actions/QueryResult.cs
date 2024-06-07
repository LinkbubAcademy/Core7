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
            output.AddErrors(this.Errors);

            if (Value != null)
            {
                switch (outputType)
                {
                    //Todo: complete all possibilities
                    case IProcessActionResult.OutputTypes.SimpleType:
                        if (className == "Guid")
                        {
                            output.Serialized = EntityMetadata.SerializeGuid(Value);
                        }
                        else if(className =="Bool")
                        {
                            output.Serialized = EntityMetadata.SerializeBool(Value);
                        }
                        else if (className == "String")
                        {
                            output.Serialized = EntityMetadata.SerializeString(Value);
                        }
                        else if (className == "Double")
                        {
                            output.Serialized = EntityMetadata.SerializeDouble(Value);
                        }
                        break;
                    case IProcessActionResult.OutputTypes.Dto:
                        output.Serialized = isCollection ? Dto.SerializeCollection((IEnumerable<object>)Value) : (Value as Dto).Serialize();
                        break;
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
