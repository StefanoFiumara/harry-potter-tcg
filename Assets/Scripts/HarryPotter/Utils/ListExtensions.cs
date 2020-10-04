using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HarryPotter.Utils
{
    public static class ListExtensions
    {
        /// <summary>
        /// Removes the top card from the list and returns it.
        /// </summary>
        public static Card TakeTop(this List<Card> list)
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

        /// <summary>
        /// Removes a random card from the list and returns it.
        /// </summary>
        public static List<Card> TakeRandom(this List<Card> list, int amount)
        {
            int resultCount = Mathf.Min(amount, list.Count);
            var result = new List<Card>(resultCount);
            
            for (int i = 0; i < resultCount; i++)
            {
                var random = Random.Range(0, list.Count);
                var card = list[random];
                
                result.Add(card);
                list.Remove(card);
            }

            return result;
        }

        public static Card TakeRandom(this List<Card> list)
        {
            if (list.Count == 0)
            {
                Debug.LogError("Attempted to TakeRandom from Empty Card List.");
                return null;
            }
            
            return list.TakeRandom(1).Single();
        }
        
        //TODO: Shuffle Deck logic
        
        /// <summary>
        /// Draws the given amount of cards, or less if there aren't enough cards in the list.
        /// </summary>
        public static List<Card> Draw(this List<Card> list, int amount)
        {
            int resultCount = Mathf.Min(amount, list.Count);
            var result = new List<Card>(resultCount);
            
            for (int i = 0; i < resultCount; i++)
            {
                var item = list.TakeTop();
                result.Add(item);
            }

            return result;
        }

        public static IEnumerable<TValue> WhereIn<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            IEnumerable<TKey> keys)
        {
            return dict
                .Where(kvp => keys.Contains(kvp.Key))
                .Select(kvp => kvp.Value);
        } 
    }
}