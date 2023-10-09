namespace Common.Lib.Infrastructure.Actions
{
	public class ActionResult : IActionResult
    {
		public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;


        public IEnumerable<string> Errors
        {
            get
            {
                return this._errors;
            }
        }

        readonly List<string> _errors = new();

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            _errors.AddRange(errors);
        }
    }
}
