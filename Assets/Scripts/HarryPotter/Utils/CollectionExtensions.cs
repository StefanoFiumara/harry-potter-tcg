using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HarryPotter.Utils
{
    public static class CollectionExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src);
        
        public static T TakeTop<T>(this List<T> list) where T : class
        {
            if (list.Count == 0)
            {
                Debug.LogError("Attempted to TakeTop from Empty Card List.");
                return null;
            }
             
            var card = list.Last();
            list.Remove(card);

            return card;
        }
        
        public static void Shuffle<T>(this List<T> list)
        {
            // NOTE: Fisher Yates shuffle -> https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int r = Random.Range(i, n);
                
                var temp = list[r];
                list[r] = list[i];
                list[i] = temp;
            }
        }
        
        public static List<T> Draw<T>(this List<T> list, int amount) 
            where T : class
        {
            int resultCount = Mathf.Min(amount, list.Count);
            var result = new List<T>(resultCount);
            
            for (int i = 0; i < resultCount; i++)
            {
                var item = list.TakeTop();
                result.Add(item);
            }

            return result;
        }

        public static List<T> TakeRandom<T>(this List<T> list, int amount)
        {
            int resultCount = Mathf.Min(amount, list.Count);
            var result = new List<T>(resultCount);
            
            for (int i = 0; i < resultCount; i++)
            {
                var random = Random.Range(0, list.Count);
                var card = list[random];
                
                result.Add(card);
                list.Remove(card);
            }

            return result;
        }
    }
}