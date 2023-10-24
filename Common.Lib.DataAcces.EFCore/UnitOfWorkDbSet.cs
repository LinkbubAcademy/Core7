using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.DataAccess.EFCore
{
    public class UnitOfWorkDbSet<T> : CachedDbSetEFWrapper<T> where T : Entity, new()

    {
        Dictionary<Guid, T>? PendingToConfirmAddToCache { get; set; }

        Dictionary<Guid, T>? PendingToConfirmUpdateToCache { get; set; }
        List<T>? PendingToConfirmRemoveFromCache { get; set; }

        public UnitOfWorkDbSet(IContextFactory contextFactory) 
            : base(contextFactory)
        {

            BelongsToUnitOfWork = true;

            PendingToConfirmAddToCache = new Dictionary<Guid, T>();
            PendingToConfirmUpdateToCache = new Dictionary<Guid, T>();
            PendingToConfirmRemoveFromCache = new List<T>();
        }

        public override Task<ISaveResult<T>> AddAsync(T entity)
        {
            var addedEntity = DbSet.Add(entity).Entity;

            if (addedEntity == null)
                return Task.FromResult((ISaveResult<T>)(new SaveResult<T>() { IsSuccess = false }));

            if (PendingToConfirmAddToCache == null)
                throw new Exception("PendingToConfirmAddToCache is null");

            PendingToConfirmAddToCache.Add(addedEntity.Id, addedEntity);
            return Task.FromResult((ISaveResult<T>)new SaveResult<T>() { IsSuccess = false, Value = addedEntity });
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

            foreach (var entity in PendingToConfirmRemoveFromCache)
            {
                CacheItems.Add(entity);
            }
        }
    }
}
