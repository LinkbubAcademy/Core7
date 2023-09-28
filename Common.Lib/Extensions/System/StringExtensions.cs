namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrEmpty(value.Trim());
        }
    }
}
