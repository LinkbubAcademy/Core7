namespace System
{
    using System.Runtime.CompilerServices;

    public static class Log
    {
        public static bool IsLogActive { get; set; }
        public static void WriteLine([CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0, string msg = "")
        {
            if (IsLogActive)
                Console.WriteLine($"{memberName}.{sourceFilePath} line {sourceLineNumber} {msg}");
        }
    }
}
