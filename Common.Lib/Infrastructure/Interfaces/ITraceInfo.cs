using Common.Lib.Authentication;

namespace Common.Lib.Infrastructure
{
    public interface ITraceInfo
    {
        AuthInfo AuthInfo { get; set; }
        string ProcessName { get; set; }
        DateTime StartedOn { get; set; }
        IEnumerable<ITraceLine> Traces { get; set; }

        ITraceLine AddTrace(string action, string ExtraInfo = "");
        IEnumerable<string> ToLinesString();
    }
}