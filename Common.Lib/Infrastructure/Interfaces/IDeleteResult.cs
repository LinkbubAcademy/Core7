using Common.Lib.Core;

namespace Common.Lib.Infrastructure
{
    public interface IDeleteResult : IActionResult
    {
        Entity DeletedEntity { get; set; }
    }

}
