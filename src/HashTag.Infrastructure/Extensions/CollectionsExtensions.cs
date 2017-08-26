using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HashTag.Infrastructure.Extensions
{
    public static class CollectionsExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            collection.ToList().ForEach(action);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var list = collection.ToList();

            var n = list.Count;
            while (n > 1)
            {
                n--;

                var k = random.Next(n + 1);
                var value = list[k];

                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static IEnumerable<T> WaitAll<T>(this IEnumerable<T> tasks)
            where T : Task
        {
            Task.WaitAll(tasks.ToArray());
            return tasks;
        }

        public static IEnumerable<Tuple<int, T>> WithIndexes<T>(this IEnumerable<T> collection)
        {
            return Enumerable.Range(0, collection.Count())
                .Zip(collection, Tuple.Create);
        }
    }
}