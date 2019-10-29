using System;
using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards
{
    [Serializable] //TODO: I don't remember why this is Serializable
    public class Card
    {
        public CardData Data;
        
        public ZoneType ZoneType;

        public List<CardAttribute> Attributes;
        
        public Card(CardData data)
        {
            Data = data;
            ZoneType = ZoneType.Deck;
            Attributes = data.Attributes; //TODO: Track modified attributes here?
        }
    }
}