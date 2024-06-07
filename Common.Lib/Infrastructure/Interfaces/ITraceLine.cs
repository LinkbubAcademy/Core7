
namespace Common.Lib.Infrastructure
{
    public interface ITraceLine
    {
        string Action { get; set; }
        string ExtraInfo { get; set; }
        DateTime StartedOn { get; set; }
        IEnumerable<ITraceLine> Substeps { get; set; }
        ITraceLine AddSubstep(string action, string ExtraInfo = "");

        IEnumerable<string> ToLineString();
    }
}