using System;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards
{
    // NOTE: Marking this class as Serializable allows unity to display its properties in the Inspector.
    [Serializable]
    public class Card
    {
        public CardData Data;

        public Zones Zone;

        public int OrderOfPlay = int.MaxValue;

        public Player Owner;

        public Card(CardData data, Player owner, Zones zone = Zones.Deck)
        {
            Data = data;
            Owner = owner;
            Zone = zone;
        }
    }
}