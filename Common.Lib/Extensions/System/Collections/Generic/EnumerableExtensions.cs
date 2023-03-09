using System.Linq.Expressions;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
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
    }
}
