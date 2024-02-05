using Common.Lib.Core;
using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure;
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
        
    }

    public class CachedDbSetEFWrapper<T> : CachedDbSetEFWrapper, IDbSet<T> where T : Entity, new()
    {
        public int NestingLevel { get; set; }

        public DbSet<T>? DbSet { get; set; }

        public CommonEfDbContext DbContext { get; set; }

        public CachedDbSetEFWrapper(IContextFactory contextFactory)
        {
            DbSetProvider = contextFactory.Resolve<IDbSetProvider>();
            DbSet = DbSetProvider.ResolveDbSet<T>();
        }

        public CachedDbSetEFWrapper(CommonEfDbContext dbContext, DbSet<T> dbSet) 
        {
            DbContext = dbContext;
            DbSet = dbSet;
        }

        public void InitForUoW()
        {
        }

        #region CRUD

        /// <summary>
        /// Add without perisistance, for uow purposes
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ISaveResult<T> Add(T entity)
        {
            entity.CleanNavigationProperties();
            var addedEntity = DbSet.Add(entity).Entity;

            var output = new SaveResult<T>()
            {
                IsSuccess = addedEntity != null,
                Value = addedEntity
            };

            return output;
        }

        public ISaveResult<T>? AddToCache(T entity)
        {
            var addToCache = CacheItems.Add(entity);
            return !addToCache.IsSuccess ? addToCache.CastoTo<T>() : default;
        }

        public virtual async Task<ISaveResult<T>> AddAsync(T entity)
        {
            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");

            var parents = entity.GetParents();
            var saveResult = Add(entity);
            if (!saveResult.IsSuccess)
                return saveResult;

            var commitResult = await DbSetProvider.SaveChangesAsync();
            entity.AssignParents(parents);

            saveResult = new SaveResult<T>()
            {
                IsSuccess = commitResult > 0,
                Value = saveResult.Value
            };

            if (saveResult.IsSuccess)
            {
                var addToCacheError = AddToCache(saveResult.Value);
                if (addToCacheError != null)
                    return addToCacheError;
            }

            return saveResult;
        }

        public ISaveResult<T> Update(T entity)
        {
            SaveResult<T> output;
            var efEntity = DbSet.Find(entity.Id);

            if (efEntity == null)
            {
                output = new SaveResult<T>();
                output.IsSuccess = false;
                output.AddError($"Entity with id {entity.Id} not found in db");
                return output;
            }

            //DbContext.Entry(efEntity).State = EntityState.Detached;

            var entityAsChanges = entity.GetChanges();
            var changes = entityAsChanges.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();
            efEntity.ApplyChanges(changes);

            var updateEntity = DbSet.Update(efEntity).Entity;

            output = new SaveResult<T>()
            {
                IsSuccess = updateEntity != null,
                Value = updateEntity
            };

            return output;
        }

        public ISaveResult<T>? UpdateToCache(T entity)
        {
            var updateToCache = CacheItems.Update(entity);
            return !updateToCache.IsSuccess ? updateToCache.CastoTo<T>() : default;
        }

        public virtual async Task<ISaveResult<T>> UpdateAsync(T entity)
        {
            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");

            entity.CleanNavigationProperties();

            var updateResult = Update(entity);

            if (!updateResult.IsSuccess)
                return updateResult;

            var commitResult = await DbSetProvider.SaveChangesAsync();

            updateResult = new SaveResult<T>
            {
                IsSuccess = commitResult != -1,
                Value = updateResult.Value
            };

            if (updateResult.IsSuccess)
            {
                var updateToCacheError = UpdateToCache(updateResult.Value);
                if (updateToCacheError != null)
                    return updateToCacheError;
            }

            return updateResult;
        }

        public IDeleteResult Delete(Guid id)
        {
            var entityToRemove = CacheItems.Find(id).Value as T;

            if (entityToRemove == null)
            {
                entityToRemove = DbSet.Find(id);

                if (entityToRemove == null)
                {
                    var error1 = new DeleteResult()
                    {
                        IsSuccess = false
                    };
                    error1.AddError($"entity with id: {id} not found");
                    return error1;
                }
                else
                {
                    entityToRemove.CleanNavigationProperties();
                }
            }

            var deleteResult = DbSet.Remove(entityToRemove).Entity != null;
            return new DeleteResult()
            {
                IsSuccess = deleteResult,
                Message = deleteResult ? string.Empty : $"DbContext cannot delete entity {id}"
            };
        }

        public virtual async Task<IDeleteResult> DeleteAsync(Guid id)
        {
            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");

            var deleteResult = Delete(id);

            if (!deleteResult.IsSuccess)
                return deleteResult;

            var commitResult = await DbSetProvider.SaveChangesAsync();                 

            var output = new DeleteResult
            {
                IsSuccess = commitResult != -1
            };

            if (output.IsSuccess)
            {
                var entityToDelete = CacheItems.Find(id).Value;
                var dependentEntities = await entityToDelete.GetDependentEntities();

                var deleteFromCache = DeleteToCache(id);

                if (deleteFromCache != null)
                {
                    if (deleteFromCache.IsSuccess)
                    {
                        foreach(var depId in dependentEntities)
                            DeleteToCache(depId);
                    }
                    return deleteFromCache;
                }
            }
            return output;
        }

        public IDeleteResult DeleteToCache(Guid id)
        {
            var deleteFromCache = CacheItems.Delete(id);
            return deleteFromCache;
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
            return CacheItems.GetBoolValueAsync<T>(Operations);
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
                // probamos para ver cuántos Requests nos devuelven
                var itemList = DbSet.ToList();
                var wlist = DbSet.Where(x => true).ToList();

                var entity = DbSet.FirstOrDefault(x => x.Id.ToString() == "4700b030-4238-47e9-92b7-fa365c9dabe4");

                foreach (var item in DbSet)
                {
                    CacheItems.Add(item);
                }
            }
        }

        
    }
}


