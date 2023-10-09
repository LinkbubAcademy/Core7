namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrEmpty(value.Trim());
        }

        public static string ToLowerCamelCase(this string value)
        {
            if (value.IsNullOrBlank()) 
                return string.Empty;

            if (value.Length == 1)
                return value.ToLower();

            return value[0].ToString().ToLower() + value.Substring(1, value.Length - 1);
        }
    }
}
