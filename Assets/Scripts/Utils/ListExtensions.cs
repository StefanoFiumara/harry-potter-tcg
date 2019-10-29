using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Game.Cards;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class ListExtensions
    {
        private static void AssertNotEmpty<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new ArgumentException("No cards in list.");
        }

        /// <summary>
        /// Removes the top card from the list and returns it.
        /// </summary>
        public static T TakeTop<T>(this IList<T> list)
        {
            list.AssertNotEmpty();

            var card = list.Last();

            list.Remove(card);

            return card;
        }

        /// <summary>
        /// Removes a random card from the list and returns it.
        /// </summary>
        public static T TakeRandom<T>(this IList<T> list)
        {
            list.AssertNotEmpty();

            var random = Random.Range(0, list.Count);
            var card = list[random];

            list.Remove(card);

            return card;

        }
    }
}