using Microsoft.EntityFrameworkCore;

namespace Common.Lib.Core.Context
{
    public interface IDbSetProvider : IDisposable
    {
        DbSet<T> ResolveDbSet<T>() where T : Entity;
        object ResolveDbSet(string typeName);
        int SaveChanges();
        Task<int> SaveChangesAsync();

    }
}
