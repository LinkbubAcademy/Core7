using System.IO;
using System.Text;

namespace System
{
    public static class StreamExtensions
    {
        public static string ToBase64(this MemoryStream ms)
        {
            var bytes = ms.ToArray();           
            return Convert.ToBase64String(bytes);
        }
    }
}
