namespace Common.Lib.Services
{
    public abstract class Dto
    {
        const string FieldSplitter = "¬";
        const string FieldSubstitute = "#~€@#~€@#~€@";
        const string ListSplitter = "|";
        const string ListSubstitute = "~#@~#@~#@~##@";

        public static TDto ElementFromString<TDto>(string input) where TDto : Dto, new()
        {
            var output = new TDto();

            var sFields = input.Split(FieldSplitter[0])
                                .Select(x => x.Replace(FieldSubstitute, FieldSplitter));

            output.Deserialize(sFields.ToArray());
            return output;
        }

        public static List<TDto> CollectionFromString<TDto>(string input) where TDto : Dto, new()
        {
            var output = new List<TDto>();
            var sRows = input.Split(ListSplitter[0])
                                .Select(x => x.Replace(ListSubstitute, ListSplitter));

            output.AddRange(sRows.Select(srow => ElementFromString<TDto>(srow)));
            return output;
        }

        public static string SerializeCollection<TDto>(IEnumerable<TDto> rows) where TDto : Dto
        {
            return string.Join(ListSplitter, rows.Select(r => r.Serialize().Replace(ListSplitter, ListSubstitute)));
        }

        public string SerializeInternal(IEnumerable<string> sFields)
        {
            var output = string.Join(FieldSplitter, sFields.Select(f => f.Replace(FieldSplitter, FieldSubstitute)));
            return output;
        }

        public abstract string Serialize();

        public abstract void Deserialize(string[] sFields);

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
