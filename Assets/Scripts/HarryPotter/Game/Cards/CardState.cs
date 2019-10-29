using System;
using HarryPotter.Enums;

namespace HarryPotter.Game.Cards
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