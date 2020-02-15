using System;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards
{
    [Serializable] //TODO: I don't remember why this is Serializable (Probably for the card creation window?) 
    public class Card
    {
        public CardData Data;
        
        public Zones Zone;

        public int OrderOfPlay = int.MaxValue;

        public Player Owner;
        
        public Card(CardData data)
        {
            Data = data;
            Zone = Zones.Deck;
        }
    }
}