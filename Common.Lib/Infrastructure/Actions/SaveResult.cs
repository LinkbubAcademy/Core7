namespace Common.Lib.Infrastructure.Actions
{
    public class SaveResult : ActionResult, ISaveResult
    {
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
    }
}
