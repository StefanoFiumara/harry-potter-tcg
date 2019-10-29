using System;
using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards
{
    [Serializable]
    public class CardState
    {
        public CardData Data;
        
        public ZoneType ZoneType;

        public List<CardAttribute> Attributes;
        
        public CardState(CardData data)
        {
            Data = data;
            ZoneType = ZoneType.Deck;
            Attributes = data.Attributes; //TODO: Track modified attributes here?
        }
    }
}