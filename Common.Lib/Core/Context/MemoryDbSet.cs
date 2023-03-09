using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Common.Lib.Core.Context
{
    public class MemoryDbSet<T> : IDbSet<T> where T : Entity, new()
    {
        public int NestingLevel { get; set; }

        IContextFactory ContextFactory { get; set; }

        List<string> Errors { get; set; } = new List<string>();

        static ConcurrentDictionary<Guid, T> Items = new ConcurrentDictionary<Guid, T>();

        public MemoryDbSet(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        #region CRUD
        public Task<ActionResult> AddAsync(T entity)
        {
            var output = new QueryResult<T>();

            if (Items.TryAdd(entity.Id, entity))
            {
                output.IsSuccess = true;
                output.Value = entity;
            }

            return Task.FromResult((ActionResult)output);
        }

        public Task<ActionResult> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
        public Task<ActionResult> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<T>> FindAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Get Single Value
        public Task<QueryResult<bool>> GetBoolValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
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
        public Task<QueryResult<int>> GetIntValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
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
        public Task<QueryResult<DateTime>> GetDateTimeValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
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
        public Task<QueryResult<double>> GetDoubleValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
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

        public Task<QueryResult<Guid>> GetGuidValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
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

        public Task<QueryResult<string>> GetStringValueAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            var source = Items.Values.AsQueryable();
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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
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
            var source = Items.Values.AsQueryable();
            var output = new QueryResult<List<TProperty>>();

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
                        ProcessOrderBy(ref source, expBuilder as IPropertySelector, vType);
                        break;
                    case QueryTypes.OrderByDesc:
                        ProcessOrderByDesc(ref source, expBuilder as IPropertySelector, vType);
                        break;

                    case QueryTypes.Select:
                        output = new QueryResult<List<TProperty>>
                                        (ProcessSelect<TProperty>(
                                            ref source,
                                            expBuilder as IPropertySelector<List<TProperty>>, vType)
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
        public Task<QueryResult<List<T>>> GetEntitiesAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            throw new NotImplementedException();
        }


        public Task<QueryResult<T>> GetEntityAsync(List<Tuple<QueryTypes, IExpressionBuilder, ValueTypes>> Operations)
        {
            throw new NotImplementedException();
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
    }
}
