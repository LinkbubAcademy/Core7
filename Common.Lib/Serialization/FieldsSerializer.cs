namespace Common.Lib.Serialization
{
    public static class FieldsSerializer
    {
        static string _splitter = "~";

        public static string Serialize(string splitter, params object[] args)
        {
            var output = string.Empty;

            for (var i = 0; i < args.Length - 1; i++)
            {
                var o = args[i];

                if (o is string s)
                {
                    output += (s + splitter);
                }
                else if (o is DateTime time)
                {
                    output += time.Ticks.ToString();
                }
                else
                {
                    output += o.ToString();
                }
            }


            return output;
        }

        public static string[] Deserialize(string splitter, string input)
        {
            return input.Split(_splitter);
        }
    }
}
