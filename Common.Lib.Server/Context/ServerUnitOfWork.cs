using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public class ServerUnitOfWork : IUnitOfWork
    {
        public void AddEntitySave(Entity entity)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
