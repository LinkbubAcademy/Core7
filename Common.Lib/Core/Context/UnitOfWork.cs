using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Infrastructure;

namespace Common.Lib.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        public List<IUoWActInfo> UowActions { get; set; } = new();

        public void AddEntitySave(Entity entity)
        {
            UowActions.Add(new UoWActInfo()
            {
                Change = entity.GetChanges(),
                ActionInfoType = ActionInfoTypes.Save
            });
        }

        public void AddEntityDelete(Entity entity)
        {
            UowActions.Add(new UoWActInfo()
            {
                Change = entity.GetChanges(),
                ActionInfoType = ActionInfoTypes.Delete
            });
        }

        public virtual async Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
