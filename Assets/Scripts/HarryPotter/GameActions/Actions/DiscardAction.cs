using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class DiscardAction : GameAction, IAbilityLoader
    {
        public Card Source { get; set; }
        
        public List<Card> DiscardedCards { get; set; }
        
        public GameAction SourceAction { get; set; }

        public override string ToString()
        {
            return $"Discard Action - {Source.Data.CardName} sends {string.Join(", ", DiscardedCards.Select(c => c.Data.CardName))} to the discard pile.";
        }

        public void Load(IContainer game, Ability ability)
        {
            Source = ability.Owner;
        
            Player = ability.Owner.Owner;
            DiscardedCards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }
    }
}