using Common.Lib.Authentication;
using Common.Lib.Infrastructure;
using System.Text;

namespace Common.Lib.Services.Protobuf
{
    public partial class ProtoTraceInfo : ITraceInfo
    {
        public ProtoTraceInfo(ITraceInfo? traceInfo)
        {
            SAuthInfo = new ProtoAuthInfo();

            if (traceInfo == null)
                IsNull = true;
            else
            {
                ProcessName = traceInfo.ProcessName;
                AuthInfo = traceInfo.AuthInfo;
                StartedOn = traceInfo.StartedOn;
                Traces = traceInfo.Traces;
            }
        }

        public DateTime StartedOn
        {
            get
            {
                return new DateTime().AddTicks(LStartedOn);
            }
            set
            {
                LStartedOn = value.Ticks;
            }
        }

        public AuthInfo AuthInfo
        {
            get
            {
                return new AuthInfo()
                {
                    UserId = SAuthInfo.UserId,
                    UserEmail = SAuthInfo.UserEmail,
                    UserToken = SAuthInfo.UserToken
                };
            }
            set
            {
                if (value != null)
                {

                    try
                    {
                        SAuthInfo.UserId = value.UserId;                        
                    }
                    catch (Exception ex)
                    {
                        SAuthInfo.UserId = default;
                    }
                    SAuthInfo.UserEmail = value.UserEmail;
                    SAuthInfo.UserToken = value.UserToken;
                }
            }
        }
        public IEnumerable<ITraceLine> Traces
        {
            get
            {
                return STraces.Select(x => x as ITraceLine);
            }
            set
            {
                value.Select(x => new ProtoTraceLine()
                {
                    Action = x.Action,
                    ExtraInfo = x.ExtraInfo,
                    StartedOn = x.StartedOn,
                    Substeps = x.Substeps
                }).DoForeach(i => STraces.Add(i));
            }
        }

        public ITraceLine AddTrace(string action, string extraInfo = "")
        {
            var traceLine = new ProtoTraceLine
            {
                AllowSubsteps = true,
                Action = action,
                ExtraInfo = extraInfo,
                StartedOn = DateTime.Now
            };

            STraces.Add(traceLine);

            return traceLine;
        }

        public IEnumerable<string> ToLinesString()
        {
            var output = new List<string>();
            output.Add($"{StartedOn.ToLongTimeString()}     {AuthInfo.UserToken}     {ProcessName}");

            foreach (var trace in Traces)
                output.AddRange(trace.ToLineString());

            return output;
        }
    }
}
