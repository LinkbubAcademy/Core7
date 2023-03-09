namespace Common.Lib.Infrastructure.Actions
{
    public class ValidationResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
