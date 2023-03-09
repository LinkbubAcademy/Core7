namespace Common.Lib.Infrastructure.Actions
{
	public class ActionResult : IActionResult
    {
		public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
