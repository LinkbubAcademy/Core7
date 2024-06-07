namespace System
{
    using Common.Lib.Infrastructure;
    using System.Runtime.CompilerServices;

    public static class Log
    {
        public static bool IsLogActive
        {
            get
            {
                return isLogActive;
            }
            set
            {
                if (!value)
                {
                    lock(logLock)
                    {
                        CurrentTraces = null;
                    }
                }
                else
                {
                    lock (logLock)
                    {
                        CurrentTraces = new List<ITraceInfo>();
                    }
                }

                isLogActive = value;
            }
        }
        static bool isLogActive;
        static object logLock = new object();   

        public static string FilteredProcess { get; set; } = string.Empty;

        public static List<ITraceInfo>? CurrentTraces { get; set; }

        public static void WriteLine([CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0, string msg = "", string process = "")
        {
            if (IsLogActive)
            {
                if (FilteredProcess == process)
                {
                    Console.WriteLine($"{memberName}.{sourceFilePath} line {sourceLineNumber} {msg}");
                }
            }
        }

        public static void AddTrace(ITraceInfo trace)
        {
            if (isLogActive)
            {
                lock (logLock)
                {
                    CurrentTraces.Add(trace);
                }
            }
        }
    }
}
