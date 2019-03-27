using System;
using HarryPotter.Enums;
using HarryPotter.Game.Data;

namespace HarryPotter.Game.State
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