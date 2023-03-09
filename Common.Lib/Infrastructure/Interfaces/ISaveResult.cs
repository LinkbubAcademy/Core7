namespace Common.Lib.Infrastructure
{
    public interface ISaveResult : IActionResult
    {
        IEnumerable<string> Errors { get; }
    }
}
