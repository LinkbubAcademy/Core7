namespace Common.Lib.Infrastructure.Actions
{
    public class ProcessActionResult : ActionResult, IProcessActionResult
    {
        public string Serialized { get; set; } = string.Empty;
        public IProcessActionResult.OutputTypes OutputType { get; set; }
        public bool IsCollection { get; set; }
        public string OutputClassName { get; set; } = string.Empty;
    }
}
