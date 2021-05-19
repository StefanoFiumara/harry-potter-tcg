using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class ReturnToHandAction : GameAction, IAbilityLoader
    {
        public List<Card> ReturnedCards { get; set; }

        public void Load(IContainer game, Ability ability)
        {
            SourceCard = ability.Owner;
        
            Player = SourceCard.Owner;
            ReturnedCards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }

        public override string ToString()
        {
            var pronoun = ReturnedCards.Count > 1 ? "their" : "its";
            return $"Return to Hand Action - {SourceCard.Data.CardName} sends {string.Join(", ", ReturnedCards.Select(c => c.Data.CardName))} back to {pronoun} owner's hand(s).";
        }
    }
}