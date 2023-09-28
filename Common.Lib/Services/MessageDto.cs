using Common.Lib.Core;

namespace Common.Lib.Services
{
    public class MessageDto : Dto
    {
        public static MessageDto ElementFromString(string input)
        {
            return ElementFromString<MessageDto>(input);
        }

        public static List<MessageDto> CollectionFromString(string input)
        {
            return CollectionFromString<MessageDto>(input);
        }

        public bool IsRead { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreationTime { get; set; }

        public override void Deserialize(string[] sfields)
        {
            IsRead = EntityMetadata.DeserializeBool(sfields[0]);
            Message = EntityMetadata.DeserializeString(sfields[1]);
            CreationTime = EntityMetadata.DeserializeDateTime(sfields[2]);
        }

        public override string Serialize()
        {
            var svalues = new string[]
            {
                EntityMetadata.SerializeBool(IsRead),
                EntityMetadata.SerializeString(Message),
                EntityMetadata.SerializeDateTime(CreationTime),
            };

            return SerializeInternal(svalues);
        }
    }
}
