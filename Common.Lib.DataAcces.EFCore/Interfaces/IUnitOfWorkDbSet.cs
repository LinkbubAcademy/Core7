using Common.Lib.Core;

namespace Common.Lib.DataAccess.EFCore
{
    public interface IUnitOfWorkDbSet
    {
        Task UpdateCache();


        Entity Find(Type type, Guid id);
    }
}
