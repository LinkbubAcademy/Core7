using Common.Lib.Authentication;
using Common.Lib.Core;
using Common.Lib.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Common.Lib.DataAccess.EFCore
{
    public class UnitOfWorkDbSet<T> : CachedDbSetEFWrapper<T>, IUnitOfWorkDbSet where T : Entity, new()
    {
        Dictionary<Guid, T> PendingToConfirmAddToCache { get; set; }
        Dictionary<Guid, T> PendingToConfirmUpdateToCache { get; set; }
        Dictionary<Guid, T> PendingToConfirmRemoveFromCache { get; set; }

        public UnitOfWorkDbSet(CommonEfDbContext dbContext, DbSet<T> dbSet) 
            : base(dbContext, dbSet)
        {
            BelongsToUnitOfWork = true;

            PendingToConfirmAddToCache = new Dictionary<Guid, T>();
            PendingToConfirmUpdateToCache = new Dictionary<Guid, T>();
            PendingToConfirmRemoveFromCache = new Dictionary<Guid, T>();
        }

        public Entity Find(Type type, Guid id)
        {
            var output = (Entity)DbContext.Find(type, id);
            return output;
        }

        public override Task<ISaveResult<T>> AddAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            var dt1 = DateTime.Now;
            var addResult = base.Add(entity);

            var dt2 = DateTime.Now;
            if (addResult.IsSuccess &&
                addResult.Value != null &&
                !PendingToConfirmAddToCache.ContainsKey(entity.Id))
            {
                PendingToConfirmAddToCache.Add(addResult.Value.Id, addResult.Value);
            }

            var dt3 = DateTime.Now;

            var dif1 = (dt2 - dt1).TotalMilliseconds;
            var dif2 = (dt3 - dt2).TotalMilliseconds;

            return Task.FromResult(addResult);
        }

        public override Task<ISaveResult<T>> UpdateAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            var updateResult = base.Update(entity);

            if (updateResult.IsSuccess &&
                updateResult.Value != null &&
                !PendingToConfirmUpdateToCache.ContainsKey(entity.Id))
            {
                PendingToConfirmUpdateToCache.Add(updateResult.Value.Id, updateResult.Value);
            }

            return Task.FromResult(updateResult);
        }

        public override Task<IDeleteResult> DeleteAsync(Guid id, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            var deleteResult = base.Delete(id);

            if (deleteResult.IsSuccess && !PendingToConfirmRemoveFromCache.ContainsKey(id))
            {
                PendingToConfirmRemoveFromCache.Add(id, null);
            }

            return Task.FromResult(deleteResult);
        }

        public void UnloadFromMemoryPendingItems()
        {
            foreach (var entity in PendingToConfirmAddToCache)
            {
                CacheItems.Delete(entity.Key);
                CacheItems.Add(entity.Value);
            }

            foreach (var entity in PendingToConfirmUpdateToCache)
            {
                CacheItems.Delete(entity.Key);
                CacheItems.Add(entity.Value);
            }

            foreach (var pair in PendingToConfirmRemoveFromCache)
            {
                CacheItems.Add(pair.Value);
            }
        }

        public async Task UpdateCache()
        {
            foreach(var e in PendingToConfirmAddToCache.Values)
                AddToCache(e);

            foreach (var e in PendingToConfirmUpdateToCache.Values)
                UpdateToCache(e);

            foreach (var id in PendingToConfirmRemoveFromCache.Keys)
            {
                var entityToDelete = CacheItems.Find(id).Value;
                var dependentEntities = await entityToDelete.GetDependentEntities();

                var result = DeleteToCache(id);

                if(result.IsSuccess)
                    foreach (var depId in dependentEntities)
                        DeleteToCache(depId);
            }
        }
    }
}