namespace Common.Lib.Infrastructure
{
    public interface IActionResult
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }
        IEnumerable<string> Errors { get; }
        public void WriteLog(string action)
        {
            Log.WriteLine($"{action} msg:{Message} errors: {string.Join(',', Errors)}");
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            ((List<string>)Errors).AddRange(errors);
        }


    }
}
