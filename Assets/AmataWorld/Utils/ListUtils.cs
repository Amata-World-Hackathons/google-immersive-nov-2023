using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace AmataWorld.Utils
{
    public static class ListUtils
    {
        private static Unity.Mathematics.Random rng = new Unity.Mathematics.Random(0x6E624EB7u);

        public static (T, int) Sample<T>(IList<T> list)
        {
            var sampleIndex = rng.NextInt(list.Count);

            return (list[sampleIndex], sampleIndex);
        }

        public static List<T> SampleUniqueWrapped<T>(IList<T> list, int count)
        {
            var results = new List<T>(count);
            var clone = list.ToList();
            var listSize = clone.Count;
            var numPasses = 1 + count / listSize;

            for (var i = 0; i < numPasses; i++)
            {
                Shuffle(clone);

                foreach (var item in clone)
                {
                    if (results.Count < count) results.Add(item);
                    else break;
                }
            }

            return results;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.NextInt(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<T> Shuffled<T>(IList<T> list)
        {
            var clone = list.ToList();
            Shuffle(clone);

            return clone;
        }
    }
}