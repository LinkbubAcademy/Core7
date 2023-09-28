using Common.Lib.Infrastructure;

namespace Common.Lib.Services.Protobuf
{
    public partial class ActionResult : IActionResult
    {
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
