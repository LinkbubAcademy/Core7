using Common.Lib.Authentication;
using System.Text;

namespace Common.Lib.Infrastructure
{
    public class TraceInfo : ITraceInfo
    {
        public AuthInfo AuthInfo { get; set; }

        public string ProcessName { get; set; } = string.Empty;

        public DateTime StartedOn { get; set; }


        public IEnumerable<ITraceLine> Traces { get; set; } = new List<ITraceLine>();

        public ITraceLine AddTrace(string action, string ExtraInfo = "")
        {
            var traceLine = new TraceLine(allowSubsteps: true);

            ((List<ITraceLine>)Traces).Add(traceLine);

            traceLine.Action = action;
            traceLine.ExtraInfo = ExtraInfo;
            traceLine.StartedOn = DateTime.Now;

            return traceLine;
        }

        public TraceInfo()
        {
            
        }

        public IEnumerable<string> ToLinesString()
        {
            var output = new List<string>();
            output.Add($"{StartedOn.ToLongTimeString()}     {AuthInfo.UserToken}     {ProcessName}");

            foreach(var trace in Traces )
                output.AddRange(trace.ToLineString());

            return output;
        }
    }

    public class TraceLine : ITraceLine
    {
        internal bool AllowSubsteps { get; set; }

        public DateTime StartedOn { get; set; }

        public string Action { get; set; } = string.Empty;

        public string ExtraInfo { get; set; } = string.Empty;

        public IEnumerable<ITraceLine> Substeps { get; set; } = new List<ITraceLine>();

        public TraceLine(bool allowSubsteps)
        {
            AllowSubsteps = allowSubsteps;
        }

        public TraceLine()
        {
                
        }

        public ITraceLine AddSubstep(string action, string ExtraInfo = "")
        {
            var traceLine = new TraceLine(allowSubsteps: false);

            ((List<ITraceLine>)Substeps).Add(traceLine);

            traceLine.Action = action;
            traceLine.ExtraInfo = ExtraInfo;
            traceLine.StartedOn = DateTime.Now;

            return traceLine;
        }

        public IEnumerable<string> ToLineString()
        {
            var output = new List<string>();            
            output.Add($"{StartedOn.ToLongTimeString()}     {Action}     {ExtraInfo}");

            foreach (var step in Substeps)
                output.AddRange(step.ToLineString());

            return output;
        }
    }
}
