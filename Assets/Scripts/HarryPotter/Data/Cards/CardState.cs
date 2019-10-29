using System;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards
{
    [Serializable]
    public class CardState
    {
        public CardData Data;
        
        public ZoneType ZoneType;

        public CardState(CardData data)
        {
            Data = data;
            ZoneType = ZoneType.Deck;
        }
    }
}