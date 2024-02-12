using Common.Lib.Core;

namespace Common.Lib.Infrastructure.Actions
{
    public class DeleteResult : ActionResult, IDeleteResult
    {
        public Entity? DeletedEntity { get; set; }
    }
}
