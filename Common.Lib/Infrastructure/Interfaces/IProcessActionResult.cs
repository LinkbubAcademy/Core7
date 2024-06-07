using Common.Lib.Core;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Services;

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
                    output = new QueryResult<Guid>() { IsSuccess = this.IsSuccess, Value = EntityMetadata.DeserializeGuid(Serialized) };
                    break;
                case "Bool":
                    output = new QueryResult<bool>() { IsSuccess = this.IsSuccess, Value = EntityMetadata.DeserializeBool(Serialized) };
                    break;
                case "String":
                    output = new QueryResult<string>() { IsSuccess = this.IsSuccess, Value = EntityMetadata.DeserializeString(Serialized) };
                    break;
                case "Double":
                    output = new QueryResult<double>() { IsSuccess = this.IsSuccess, Value = EntityMetadata.DeserializeDouble(Serialized) };
                    break;
                default:
                    Log.WriteLine(OutputClassName);
                    return new QueryResult();

            }

            output.IsSuccess = IsSuccess;
            output.IsMaintenanceModeOn = IsMaintenanceModeOn;
            output.Message = Message;
            output.AddErrors(Errors);

            return output;
        }

        public QueryResult<TDto> ToQueryResultDto<TDto>() where TDto : Dto, new()
        {
            var output = new QueryResult<TDto>()
            {
                IsSuccess = this.IsSuccess,
                IsMaintenanceModeOn = this.IsMaintenanceModeOn,
                Message = this.Message,
                Value = Dto.ElementFromString<TDto>(Serialized)
            };

            return output;
        }

        public QueryResult<List<TDto>> ToQueryResultListDto<TDto>() where TDto : Dto, new()
        {
            var output = new QueryResult<List<TDto>>()
            {
                IsSuccess = this.IsSuccess,
                IsMaintenanceModeOn = this.IsMaintenanceModeOn,
                Message = this.Message,
                Value = Dto.CollectionFromString<TDto>(Serialized)
            };

            return output;
        }
    }
}
