using Common.Lib.Core;

namespace Common.Lib.Services
{
    public class KeyValueDto : Dto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public static KeyValueDto ElementFromString(string input)
        {
            return ElementFromString<KeyValueDto>(input);
        }

        public static List<KeyValueDto> CollectionFromString(string input)
        {
            return CollectionFromString<KeyValueDto>(input);
        }

        public KeyValueDto()
        {
            
        }

        public KeyValueDto(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override void Deserialize(string[] sfields)
        {
            Key = EntityMetadata.DeserializeString(sfields[0]);
            Value = EntityMetadata.DeserializeString(sfields[1]);
        }

        public override string Serialize()
        {
            var svalues = new string[]
            {
                EntityMetadata.SerializeString(Key),
                EntityMetadata.SerializeString(Value)
            };

            return SerializeInternal(svalues);
        }
    }

    public class KeyValueDto<TKey, TValue>(TKey key, TValue value)
    {
        public TKey Key { get; set; } = key;

        public TValue Value { get; set; } = value;

        public static KeyValueDto<TKey, TValue> FromVeyValueDto(KeyValueDto input, Func<string, TKey> keyConverter, Func<string, TValue> valueConverter)
        {
            return new KeyValueDto<TKey, TValue>(keyConverter(input.Key), valueConverter(input.Value));
        }
    }
}
