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
            return null;
        }

        public virtual async Task<ISaveResult<T>> AddAsync(T entity)
        {
            var addedEntity = DbSet.Add(entity).Entity;
            var output = new SaveResult<T>();

            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");
            
            var saveResult = await DbSetProvider.SaveChangesAsync();
            output.IsSuccess = saveResult != -1;

            if (output.IsSuccess)
            {
                var addToCache = CacheItems.Add(addedEntity);
                if (!addToCache.IsSuccess)
                    return addToCache.CastoTo<T>();                
            }

            output.Value = addedEntity;
            return output;
        }

        public ActionResult Update(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ISaveResult<T>> UpdateAsync(T entity)
        {
            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");

            var output = new SaveResult<T>();
            var efEntity = await DbSet.FindAsync(entity.Id);

            if (efEntity == null)
            {
                output.IsSuccess = false;
                output.AddError($"Entity with id {entity.Id} not found in db");
                return output;
            }

            var entityAsChanges = entity.GetChanges();
            var changes = entityAsChanges.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();
            efEntity.ApplyChanges(changes);

            var updateEntity = DbSet.Update(efEntity).Entity;
            var saveResult = await DbSetProvider.SaveChangesAsync();
            output.IsSuccess = saveResult != -1;

            if (output.IsSuccess)
            {
                var updateToCache = CacheItems.Update(updateEntity);
                if (!updateToCache.IsSuccess)
                    return updateToCache.CastoTo<T>();
            }

            output.Value = updateEntity;
            return output;
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IDeleteResult> DeleteAsync(Guid id)
        {
            if (DbSetProvider == null)
                throw new Exception("DbSetProvider is null");

            T entityToRemove;

            if (CacheItems.Find(id).Value == null)
            {
                entityToRemove = await DbSet.FindAsync(id);

                if (entityToRemove == null)
                {
                    var error1 = new DeleteResult()
                    {
                        IsSuccess = false
                    };
                    error1.AddError($"entity with id: {id} not found");
                    return error1;
                }
            }
            else
            {
                entityToRemove = await DbSet.FindAsync(id);
            }

            var result = DbSet.Remove(entityToRemove).Entity != null;
            //todo: handle possible error

            var efSaveChangesResult = await DbSetProvider.SaveChangesAsync();
            var output = new DeleteResult();
            output.IsSuccess = efSaveChangesResult != -1;

            if (output.IsSuccess)
            {
                var deleteFromCache = CacheItems.Delete(id);
                if (!deleteFromCache.IsSuccess)
                    return deleteFromCache;
            }
            return output;
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
                foreach (var item in DbSet)
                {
                    CacheItems.Add(item);
                }
            }
        }

        
    }
}


