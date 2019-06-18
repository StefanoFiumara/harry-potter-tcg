using System;
using HarryPotter.Enums;

namespace HarryPotter.Game.Cards
{
    [Serializable]
    public class CardState
    {
        public CardData Data;
        
        public Zone Zone;

        public CardState(CardData data)
        {
            Data = data;
            Zone = Zone.Deck;
        }
    }
}