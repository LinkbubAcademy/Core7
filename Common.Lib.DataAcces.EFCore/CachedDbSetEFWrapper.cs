using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Common.Lib.DataAccess.EFCore
{
    public abstract class CachedDbSetEFWrapper
    {
        public IContextFactory ContextFactory { get; set; }
        public IDbSetProvider? DbSetProvider { get; set; }

        public static MemoryDbSet<Entity>? CacheItems { get; set; }
        public bool BelongsToUnitOfWork { get; set; }

        public abstract void InitCacheItems();

        public abstract void UnloadFromMemoryPendingItems();
        
    }

    public class CachedDbSetEFWrapper<T> : CachedDbSetEFWrapper, IDbSet<T> where T : Entity, new()
    {
        Dictionary<Guid, T>? PendingToConfirmAddToCache { get; set; }

        Dictionary<Guid, T>? PendingToConfirmUpdateToCache { get; set; }

        List<T>? PendingToConfirmRemoveFromCache { get; set; }

        public int NestingLevel { get; set; }

        public DbSet<T>? DbSet { get; set; }

        public CachedDbSetEFWrapper(IContextFactory contextFactory)
        {
            DbSetProvider = contextFactory.Resolve<IDbSetProvider>();
            DbSet = DbSetProvider.ResolveDbSet<T>();
        }

        public CachedDbSetEFWrapper(DbSet<T> dbSet) 
        {
            DbSet = dbSet;
        }

        public void InitForUoW()
        {
            BelongsToUnitOfWork = true;

            PendingToConfirmAddToCache = new Dictionary<Guid, T>();
            PendingToConfirmUpdateToCache = new Dictionary<Guid, T>();
            PendingToConfirmRemoveFromCache = new List<T>();
        }

        #region CRUD

        /// <summary>
        /// Add without perisistance, for uow purposes
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ActionResult Add(T entity)
        {
            var addedEntity = DbSet.Add(entity).Entity;

            if (addedEntity == null)
                return new ActionResult() { IsSuccess = false };
                            
            if (PendingToConfirmAddToCache == null)
                throw new Exception("PendingToConfirmAddToCache is null");
                
            PendingToConfirmAddToCache.Add(addedEntity.Id, addedEntity);
            return new ActionResult() { IsSuccess = false };
        }

        public async Task<ActionResult> AddAsync(T entity)
        {
            var addedEntity = DbSet.Add(entity).Entity;
            var output = new ActionResult();

            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");
            
            var saveResult = await DbSetProvider.SaveChangesAsync();
            output.IsSuccess = saveResult != -1;

            if (output.IsSuccess)
            {
                var addToCache = CacheItems.Add(addedEntity);
                if (!addToCache.IsSuccess)
                    return addToCache;
            }

            return output;

        }

        public ActionResult Update(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public QueryResult<T> Find(Guid id)
        {
            return CacheItems.Find<T>(id);
        }

        public async Task<QueryResult<T>> FindAsync(Guid id)
        {
            return await CacheItems.FindAsync<T>(id);
        }

        #endregion

        #region Get Single Value
        public Task<QueryResult<bool>> GetBoolValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetBoolValueAsync(Operations);
        }
        public Task<QueryResult<int>> GetIntValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetIntValueAsync<T>(Operations);
        }

        public Task<QueryResult<byte[]>> GetBytesValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetBytesValueAsync(Operations);
        }
        public Task<QueryResult<DateTime>> GetDateTimeValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetDateTimeValueAsync(Operations);
        }
        public Task<QueryResult<double>> GetDoubleValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetDoubleValueAsync(Operations);
        }

        public Task<QueryResult<Guid>> GetGuidValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return GetGuidValueAsync(Operations);
        }

        public Task<QueryResult<string>> GetStringValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return GetStringValueAsync(Operations);
        }


        #endregion

        #region Get Collection of Values
        public Task<QueryResult<List<TProperty>>> GetValuesAsync<TProperty>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetValuesAsync<TProperty>(Operations);
        }

        #endregion

        #region Get Entities
        public Task<QueryResult<List<T>>> GetEntitiesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetEntitiesAsync<T>(Operations);
        }

        public Task<QueryResult<T>> GetEntityAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            return CacheItems.GetEntityAsync<T>(Operations);
        }


        #endregion

        #region Process

        void ProcessWhere(ref IQueryable<T> source, IQueryExpression<bool>? expression)
        {
            var condition = expression?.CreateExpression<T>();
            source = source.Where(condition ?? (x => true)).AsQueryable();
        }

        void ProcessOrderBy(ref IQueryable<T> source, IPropertySelector selector, ValueTypes vType)
        {
            switch (vType)
            {
                case ValueTypes.String:
                    source = source
                        .OrderBy((selector as IPropertySelector<string>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Int:
                    source = source
                        .OrderBy((selector as IPropertySelector<int>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Bool:
                    source = source
                        .OrderBy((selector as IPropertySelector<bool>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.ByteArray:
                    source = source
                        .OrderBy((selector as IPropertySelector<byte[]>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Double:
                    source = source
                        .OrderBy((selector as IPropertySelector<double>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.DateTime:
                    source = source
                        .OrderBy((selector as IPropertySelector<DateTime>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.ReferencedImage:
                    source = source
                        .OrderBy((selector as IPropertySelector<string>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Guid:
                    source = source
                        .OrderBy((selector as IPropertySelector<Guid>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                default:
                    break;
            }
        }

        void ProcessOrderByDesc(ref IQueryable<T> source, IPropertySelector selector, ValueTypes vType)
        {
            switch (vType)
            {
                case ValueTypes.String:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<string>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Int:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<int>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Bool:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<bool>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.ByteArray:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<byte[]>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Double:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<double>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.DateTime:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<DateTime>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.ReferencedImage:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<string>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                case ValueTypes.Guid:
                    source = source
                        .OrderByDescending((selector as IPropertySelector<Guid>).CreateSelector<T>())
                        .AsQueryable();
                    break;
                default:
                    break;
            }
        }

        IEnumerable<TProperty>? ProcessSelect<TProperty>(ref IQueryable<T> source, IPropertySelector selector, ValueTypes vType)
        {
            switch (vType)
            {
                case ValueTypes.ReferencedImage:
                case ValueTypes.String:
                    return source
                        .Select((selector as IPropertySelector<string>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                case ValueTypes.Int:
                    return source
                        .Select((selector as IPropertySelector<int>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                case ValueTypes.Bool:
                    return source
                        .Select((selector as IPropertySelector<bool>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                case ValueTypes.ByteArray:
                    return source
                        .Select((selector as IPropertySelector<byte[]>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                case ValueTypes.Double:
                    return source
                        .Select((selector as IPropertySelector<double>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                case ValueTypes.DateTime:
                    return source
                        .Select((selector as IPropertySelector<DateTime>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                case ValueTypes.Guid:
                    return source
                        .Select((selector as IPropertySelector<Guid>)?.CreateSelector<T>()) as IEnumerable<TProperty>;

                default:
                    break;
            }
            return null;
        }


        #endregion

        Expression<Func<T, bool>>? GetCondition(IExpressionBuilder expBuilder)
        {
            var expGroup = expBuilder as ExpressionsGroup;

            var expBoolBuilder = expGroup != null ?
                                    expGroup.Expressions?.CombineExpressions<T>() :
                                    expBuilder as IQueryExpression<bool>;

            var condition = expBoolBuilder != null ?
                                expBoolBuilder?.CreateExpression<T>() :
                                (x => true);

            return condition;
        }

        IPropertySelector CastToPropertySelecter(IExpressionBuilder expBuilder)
        {
            var propSel = expBuilder as IPropertySelector;

            if (propSel != null)
                return propSel;

            throw new Exception("builder is not a IPropertySelector");
        }

        public override void InitCacheItems()
        {
            if (CacheItems == null)
            {
                CacheItems = new MemoryDbSet<Entity>();
            }

            if (CacheItems.CountItems<T>() == 0)
            {
                foreach (var item in DbSet)
                {
                    CacheItems.Add(item);
                }
            }
        }

        public override void UnloadFromMemoryPendingItems()
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


