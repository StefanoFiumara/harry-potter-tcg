using System;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards
{
    public class Card
    {
        public CardData Data { get; }

        public Zones Zone { get; set; }

        public int OrderOfPlay { get; set; } = int.MaxValue;

        public Player Owner { get; set; }

        public Card(CardData data, Player owner)
        {
            Data = data;
            Owner = owner;
            Zone = Zones.Deck;
        }
    }
}