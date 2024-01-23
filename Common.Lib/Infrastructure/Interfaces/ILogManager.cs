using Common.Lib.Core.Context;
using Common.Lib.Core.Tracking;

namespace Common.Lib.Infrastructure
{
    public interface ILogManager
    {
        Task<bool> RegisterChangesAsync(IEntityInfo entityInfo, string user, bool isNew = false);
        void RegisterChanges(IUnitOfWork uow, IEntityInfo entityInfo, string user);
    }
}
