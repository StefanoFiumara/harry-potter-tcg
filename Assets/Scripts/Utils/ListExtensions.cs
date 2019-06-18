using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Game.Cards;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class ListExtensions
    {
        private static void AssertNotEmpty(this List<CardView> list)
        {
            if (list.Count == 0) throw new ArgumentException("No cards in list.");
        }

        /// <summary>
        /// Removes the top card from the list and returns it.
        /// </summary>
        public static CardView TakeTopCard(this List<CardView> list)
        {
            list.AssertNotEmpty();

            var card = list.Last();

            list.Remove(card);

            return card;
        }

        /// <summary>
        /// Removes a random card from the list and returns it.
        /// </summary>
        public static CardView TakeRandomCard(this List<CardView> list)
        {
            list.AssertNotEmpty();

            var random = Random.Range(0, list.Count);
            var card = list[random];

            list.Remove(card);

            return card;

        }
    }
}