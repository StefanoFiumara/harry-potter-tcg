using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions
{
    public class DrawCardsAction : GameAction
    {
        public bool UsePlayerAction { get; }
        public int Amount { get; }
        public List<Card> Cards { get; set; }

        public DrawCardsAction(Player player, int amount, bool usePlayerAction = false)
        {
            UsePlayerAction = usePlayerAction;
            Player = player;
            Amount = amount;
        }
    }
}