using System.Linq;
using System.Linq.Expressions;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return await Task.FromResult(item);
            }
        }

        public static ValueTask<List<T>> ToListAsync<T>(this IEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            return source.ToAsyncEnumerable().ToListAsync(cancellationToken);
        }

        public static ValueTask<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new Exception(nameof(source));

            if (source is IAsyncIListProvider<TSource> listProvider)
                return listProvider.ToListAsync(cancellationToken);

            return Core(source, cancellationToken);

            static async ValueTask<List<TSource>> Core(IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
            {
                var list = new List<TSource>();

                await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    list.Add(item);
                }

                return list;
            }
        }
        public static void DoForeach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static bool None<T>(this IEnumerable<T> source, Expression<Func<T, bool>> predicate)
        {
            var condition = predicate.Compile();
            foreach (var item in source)
            {
                if (condition(item))
                    return false;
            }

            return true;
        }

        public static async Task<IEnumerable<T1>> SelectManyAsync<T, T1>(this IEnumerable<T> enumeration, Func<T, Task<IEnumerable<T1>>> func)
        {
            return (await Task.WhenAll(enumeration.Select(func))).SelectMany(s => s);
        }
    }
}
