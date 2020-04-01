using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.PlayerActions
{
    public class DamageAction : GameAction
    {
        public Card Source { get; }
        public Player Target { get; }
        public int Amount { get; }
        
        public List<Card> Cards { get; set; }

        public DamageAction(Card source, Player target, int amount)
        {
            Source = source;
            Target = target;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"DamageAction - {Source.Data.CardName} does {Amount} damage to Player {Target.Index}";
        }
    }
}