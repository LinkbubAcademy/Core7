namespace Common.Lib.DataAccess.EFCore
{
    public interface IUnitOfWorkDbSet
    {
        Task UpdateCache();
    }
}
