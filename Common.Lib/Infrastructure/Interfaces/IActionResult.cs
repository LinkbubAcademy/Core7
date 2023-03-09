namespace Common.Lib.Infrastructure
{
    public interface IActionResult
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }
    }
}
