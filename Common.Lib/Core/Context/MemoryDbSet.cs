using Common.Lib.Authentication;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure;
using Common.Lib.Infrastructure.Actions;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace Common.Lib.Core.Context
{
    public class MemoryDbSet<T> : IDbSet<T> where T : Entity, new()
    {
        public int NestingLevel { get; set; }

        List<string> Errors { get; set; } = new List<string>();

        static ConcurrentDictionary<Guid, T> Items = new ConcurrentDictionary<Guid, T>();


        public MemoryDbSet()
        {
        }

        public IEnumerable<T> GetReader()
        { 
            return Items.Values;
        }

        public void Clear()
        {
            Items = new ConcurrentDictionary<Guid, T>();
        }

        #region CRUD

        public ISaveResult<T> Add(T entity)
        {
            var output = new SaveResult<T>();

            if (Items.TryAdd(entity.Id, entity))
            {
                output.IsSuccess = true;
                output.Value = entity;
            }

            return output;
        }

        public Task<ISaveResult<T>> AddAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            return Task.FromResult(Add(entity));
        }

        public ISaveResult<T> Update(T entity)
        {
            var output = new SaveResult<T>();
            if (!Items.ContainsKey(entity.Id))
            {
                output.IsSuccess = false;
                output.AddError($"Entity with id {entity.Id} is not in the cache");
                return output;
            }

            var existingEntity = Items[entity.Id];


            var entityAsChanges = entity.GetChanges();
            var changes = entityAsChanges.GetChangeUnits().OrderBy(x => x.MetadataId).ToList();
            existingEntity.ApplyChanges(changes);
            output.IsSuccess = true;
            output.Value = existingEntity;

            //if (Items.TryUpdate(entity.Id, entity, existingEntity))
            //{
            //    output.IsSuccess = true;
            //    output.Value = existingEntity;
            //}
            //else
            //{
            //    output.IsSuccess = false;
            //    output.AddError($"error updating Entity with id {entity.Id} in the cache");
            //}

            return output;
        }


        public Task<ISaveResult<T>> UpdateAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            return Task.FromResult(Update(entity));
        }
        public Task<IDeleteResult> DeleteAsync(Guid id, AuthInfo? info = null, ITraceInfo? trace = null)
        {
            return Task.FromResult(Delete(id));
        }

        public IDeleteResult Delete(Guid id)
        {
            var output = new DeleteResult();
            
            if (!Items.ContainsKey(id))
            {
                output.AddError($"entity with {id} not found in cache");
                return output;
            }

            if (!Items.Remove(id, out T? entityToDelete))
            {
                output.AddError($"entity with {id} cannot be removed but it is in the cache");
                return output;
            }

            output.IsSuccess = true;
            output.DeletedEntity = entityToDelete;

            return output;
        }

        public QueryResult<T> Find(Guid id)
        {
            if (Items.TryGetValue(id, out var result))
                return new QueryResult<T>()
                {
                    IsSuccess = true,
                    Value = result
                };

            return new QueryResult<T>()
            {
                IsSuccess = false,
                Value = null
            };
        }

        public QueryResult<T1> Find<T1>(Guid id) where T1 : T
        {
            if (Items.TryGetValue(id, out var result))
                return new QueryResult<T1>()
                {
                    IsSuccess = true,
                    Value = (T1)result
                };

            return new QueryResult<T1>()
            {
                IsSuccess = false,
                Value = null
            };
        }

        public Task<QueryResult<T>> FindAsync(Guid id)
        {
            if (Items.TryGetValue(id, out var result))
                return Task.FromResult(new QueryResult<T>()
                {
                    IsSuccess = true,
                    Value = result
                });

            return Task.FromResult(new QueryResult<T>()
            { 
                IsSuccess = false, 
                Value = null 
            });
        }

        public Task<QueryResult<T1>> FindAsync<T1>(Guid id) where T1 : Entity, new()
        {
            if (Items.TryGetValue(id, out var result))
            {
                if (result is T1)
                    return Task.FromResult(new QueryResult<T1>()
                    {
                        IsSuccess = true,
                        Value = result as T1
                    });
               
                var e1 = new QueryResult<T1>()
                {
                    IsSuccess = false
                };

                e1.AddError($"entity with id {id} is not {typeof(T1).FullName}");
                return Task.FromResult(e1);

            }

            var e2 = new QueryResult<T1>()
            {
                IsSuccess = false
            };

            e2.AddError($"not found entity with id {id}");
            return Task.FromResult(e2);
        }

        #endregion

        #region Get Single Value
        public Task<QueryResult<bool>> GetBoolValueAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            return GetBoolValueAsync(source, Operations);
        }

        public Task<QueryResult<bool>> GetBoolValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<T>().AsQueryable();
            return GetBoolValueAsync(source, Operations);
        }

        Task<QueryResult<bool>> GetBoolValueAsync(IQueryable<T> source, List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var output = new QueryResult<bool>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.Any:
                        output = new QueryResult<bool>(source.Any(condition));
                        break;
                    case QueryTypes.All:
                        output = new QueryResult<bool>(source.All(condition));
                        break;
                    case QueryTypes.None:
                        output = new QueryResult<bool>(source.None(condition));
                        break;
                    case QueryTypes.SelectOne:
                        var value = ProcessSelect<bool>(ref source, expBuilder as IPropertySelector, vType).First();
                        output = new QueryResult<bool>(value);
                        break;

                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }


        public Task<QueryResult<int>> GetIntValueAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            return GetIntValueAsync(source, Operations);
        }

        public Task<QueryResult<int>> GetIntValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<T>().AsQueryable();
            return GetIntValueAsync(source, Operations);
        }

        Task<QueryResult<int>> GetIntValueAsync(IQueryable<T> source, List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var output = new QueryResult<int>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                var expIntBuilder = expBuilder as IPropertySelector<int>;

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.Count:
                        output = new QueryResult<int>(source.Count(condition));
                        break;
                    case QueryTypes.Max:
                        output = new QueryResult<int>(source.Max(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.Min:
                        output = new QueryResult<int>(source.Min(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.Sum:
                        output = new QueryResult<int>(source.Sum(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.SelectOne:
                        var value = ProcessSelect<int>(ref source, expBuilder as IPropertySelector<int>, vType).First();
                        output = new QueryResult<int>(value);
                        break;
                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }


        public Task<QueryResult<byte[]>> GetBytesValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            throw new NotImplementedException();
        }
        
        
        public Task<QueryResult<DateTime>> GetDateTimeValueAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            return GetDateTimeValueAsync(source, Operations);
        }

        public Task<QueryResult<DateTime>> GetDateTimeValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<T>().AsQueryable();
            return GetDateTimeValueAsync(source, Operations);
        }

        Task<QueryResult<DateTime>> GetDateTimeValueAsync(IQueryable<T> source, List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var output = new QueryResult<DateTime>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                var expDateTimeBuilder = expBuilder as IPropertySelector<DateTime>;

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.Max:
                        output = new QueryResult<DateTime>(source.Max(expDateTimeBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.Min:
                        output = new QueryResult<DateTime>(source.Min(expDateTimeBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.SelectOne:
                        var value = ProcessSelect<DateTime>(ref source, expBuilder as IPropertySelector, vType).First();
                        output = new QueryResult<DateTime>(value);
                        break;
                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }


        public Task<QueryResult<double>> GetDoubleValueAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            return GetDoubleValueAsync(source, Operations);
        }

        public Task<QueryResult<double>> GetDoubleValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<T>().AsQueryable();
            return GetDoubleValueAsync(source, Operations);
        }

        public Task<QueryResult<double>> GetDoubleValueAsync(IQueryable<T> source, List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var output = new QueryResult<double>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                var expIntBuilder = expBuilder as IPropertySelector<int>;

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.Max:
                        output = new QueryResult<double>(source.Max(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.Min:
                        output = new QueryResult<double>(source.Min(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.Sum:
                        output = new QueryResult<double>(source.Sum(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.Avg:
                        output = new QueryResult<double>(source.Sum(expIntBuilder.CreateSelector<T>()));
                        break;
                    case QueryTypes.SelectOne:
                        var value = ProcessSelect<double>(ref source, expBuilder as IPropertySelector, vType).First();
                        output = new QueryResult<double>(value);
                        break;
                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }

        
        public Task<QueryResult<Guid>> GetGuidValueAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            return GetGuidValueAsync(source, Operations);
        }

        public Task<QueryResult<Guid>> GetGuidValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<T>().AsQueryable();
            return GetGuidValueAsync(source, Operations);
        }

        Task<QueryResult<Guid>> GetGuidValueAsync(IQueryable<T> source, List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var output = new QueryResult<Guid>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.SelectOne:
                        var value = ProcessSelect<Guid>(ref source, expBuilder as IPropertySelector<Guid>, vType).First();
                        output = new QueryResult<Guid>(value);
                        break;
                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }

        public Task<QueryResult<string>> GetStringValueAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            return GetStringValueAsync(source, Operations);
        }

        public Task<QueryResult<string>> GetStringValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.OfType<T>().AsQueryable();
            return GetStringValueAsync(source, Operations);
        }

        Task<QueryResult<string>> GetStringValueAsync(IQueryable<T> source, List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var output = new QueryResult<string>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.SelectOne:
                        var value = ProcessSelect<string>(ref source, expBuilder as IPropertySelector<string>, vType).First();
                        output = new QueryResult<string>(value);
                        break;
                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }

        #endregion

        #region Get Collection of Values
        public Task<QueryResult<List<TProperty>>> GetValuesAsync<TProperty>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable() ?? 
                throw new ArgumentNullException("source cannot be null");

            var output = new QueryResult<List<TProperty>>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);
                IPropertySelector propSel = null;

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        propSel = CastToPropertySelecter(expBuilder);
                        ProcessOrderBy(ref source, propSel);
                        break;

                    case QueryTypes.OrderByDesc:
                        propSel = CastToPropertySelecter(expBuilder);
                        ProcessOrderByDesc(ref source, propSel);
                        break;

                    case QueryTypes.Select:
                        propSel = CastToPropertySelecter(expBuilder);
                        output = new QueryResult<List<TProperty>>(ProcessSelect<TProperty>(
                                            ref source, propSel, propSel.PropertyType)
                                        .ToList());
                        break;

                    default:
                        continue;
                }
            }

            return Task.FromResult(output);
        }

        #endregion

        #region Get Entities

        public int CountItems<TOut>() where TOut : T
        {
            return Items.Values.OfType<TOut>().Count();
        }

        public Task<QueryResult<List<TOut>>> GetEntitiesAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations) where TOut : T
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            var z = source.ToList();
            var output = new QueryResult<List<TOut>>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                switch (queryType)
                {
                    case QueryTypes.Where:
                        if (expBuilder is ExpressionsGroup)
                            ProcessWhere(ref source, expBuilder as ExpressionsGroup);
                        else
                            ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;

                    default:
                        continue;
                }
            }

            output.IsSuccess = true;
            var a = source.ToList();
            output.Value = source.OfType<TOut>().ToList();

            return Task.FromResult(output);
        }

        public Task<QueryResult<TOut>> GetEntityAsync<TOut>(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations) where TOut : T
        {
            var source = Items.Values.OfType<TOut>().Cast<T>().AsQueryable();
            var output = new QueryResult<TOut>();

            T e = null;

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                switch (queryType)
                {
                    case QueryTypes.Where:
                        if (expBuilder is ExpressionsGroup)
                            ProcessWhere(ref source, expBuilder as ExpressionsGroup);
                        else
                            ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;

                    case QueryTypes.Find:
                    case QueryTypes.FirstOrDefault:
                        output.IsSuccess = true;
                        e = condition == null ?
                                            source.OfType<TOut>().Cast<T>().FirstOrDefault() :
                                            source.OfType<TOut>().Cast<T>().FirstOrDefault(condition);
                        output.Value = e as TOut;
                        return Task.FromResult(output);

                    case QueryTypes.LastOrDefault:
                        output.IsSuccess = true;
                        e = condition == null ?
                                            source.OfType<TOut>().Cast<T>().LastOrDefault() :
                                            source.OfType<TOut>().Cast<T>().LastOrDefault(condition);

                        output.Value = e as TOut;
                        return Task.FromResult(output);

                    default:

                        continue;
                }
            }


            output.IsSuccess = false;
            output.Message = "Querying an entity must end with single entity query (eg. FirstOrDefault)";

            return Task.FromResult(output);
        }


        public Task<QueryResult<List<T>>> GetEntitiesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
            var output = new QueryResult<List<T>>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                switch (queryType)
                {
                    case QueryTypes.Where:
                        if (expBuilder is ExpressionsGroup)
                            ProcessWhere(ref source, expBuilder as ExpressionsGroup);
                        else
                            ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;

                    default:
                        continue;
                }
            }

            output.IsSuccess = true;
            output.Value = source.ToList();

            return Task.FromResult(output);
        }

        public Task<QueryResult<T>> GetEntityAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
            var output = new QueryResult<T>();

            foreach (var tuple in Operations)
            {
                var queryType = tuple.Item1;
                var expBuilder = tuple.Item2;
                var vType = tuple.Item3;

                var condition = GetCondition(expBuilder);

                switch (queryType)
                {
                    case QueryTypes.Where:
                        ProcessWhere(ref source, expBuilder as IQueryExpression<bool>);
                        break;
                    case QueryTypes.OrderBy:
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector);
                        break;
                    case QueryTypes.Find: //Todo: place Find in its proper place to get by dictionary key
                    case QueryTypes.FirstOrDefault:
                        output.IsSuccess = true;
                        output.Value = condition == null ? 
                                            source.FirstOrDefault() : 
                                            source.FirstOrDefault(condition);

                        return Task.FromResult(output);

                    case QueryTypes.LastOrDefault:
                        output.IsSuccess = true;
                        output.Value = condition == null ?
                                            source.LastOrDefault() :
                                            source.LastOrDefault(condition);

                        return Task.FromResult(output);

                    default:
                        
                        continue;
                }
            }


            output.IsSuccess = false;
            output.Message = "Querying an entity must end with single entity query (eg. FirstOrDefault)";

            return Task.FromResult(output);
        }


        #endregion

        #region Process

        void ProcessWhere(ref IQueryable<T> source, ExpressionsGroup? expBuilder)
        {
            //var condition = expGroup.Expressions?.CombineExpressions<T>();
            //var c = condition.CreateExpression<T>();
            var condition = expBuilder == null ? null : GetCondition(expBuilder);
            source = source.Where(condition ?? (x => true)).AsQueryable();
        }

        void ProcessWhere(ref IQueryable<T> source, IQueryExpression<bool>? expression)
        {
            var condition = expression?.CreateExpression<T>();
            source = source.Where(condition ?? (x => true)).AsQueryable();
        }

        void ProcessOrderBy(ref IQueryable<T> source, IPropertySelector selector)
        {            
            switch (selector.PropertyType)
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

        void ProcessOrderByDesc(ref IQueryable<T> source, IPropertySelector selector)
        {
            switch (selector.PropertyType)
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
    }
}
