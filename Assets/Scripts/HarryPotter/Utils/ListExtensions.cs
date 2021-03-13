using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Views;
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
        /// Removes a random element from the list and returns it.
        /// </summary>
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

        public static Card TakeRandom(this List<Card> list)
        {
            if (list.Count == 0)
            {
                Debug.LogError("Attempted to TakeRandom from Empty Card List.");
                return null;
            }
            
            return list.TakeRandom(1).Single();
        }

        public static void Highlight(this IEnumerable<CardView> cards, Color color)
        {
            foreach (var cardView in cards)
            {
                cardView.Highlight(color);
            }
        }

        /// <summary>
        /// Shuffles the list
        /// </summary>
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
        
        /// <summary>
        /// Draws the given amount of list, or less if there aren't enough list in the list.
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

        public static IOrderedEnumerable<CardData> SortCards(this IEnumerable<CardData> cards)
        {
            return cards.OrderBy(c => c.GetDataAttribute<LessonCost>()?.Type)
                .ThenBy(c => c.Type)
                .ThenBy(c => c.GetDataAttribute<LessonCost>()?.Amount)
                .ThenBy(c => c.CardName);
        }
    }
}