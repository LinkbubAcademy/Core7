using Common.Lib.Infrastructure;

namespace Common.Lib.Services.Protobuf
{
    public partial class ProtoTraceLine : ITraceLine
    {
        internal bool AllowSubsteps { get; set; }
        public ProtoTraceLine(ITraceLine line)
        {
            Action = line.Action;
            ExtraInfo = line.ExtraInfo;
            StartedOn = line.StartedOn;
            Substeps = line.Substeps;
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
        public IEnumerable<ITraceLine> Substeps
        {
            get
            {
                return SSubsteps.Select(x => x as ITraceLine).ToList();
            }
            set
            {
                value.Select(x => new ProtoTraceLine()
                {
                    Action = x.Action,
                    ExtraInfo = x.ExtraInfo,
                    StartedOn = x.StartedOn,
                    Substeps = x.Substeps
                }).DoForeach(i => SSubsteps.Add(i));
            }
        }

        public ITraceLine AddSubstep(string action, string extraInfo = "")
        {
            var traceLine = new ProtoTraceLine
            {
                AllowSubsteps = false,
                Action = action,
                ExtraInfo = extraInfo,
                StartedOn = DateTime.Now
            };

            SSubsteps.Add(traceLine);


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
