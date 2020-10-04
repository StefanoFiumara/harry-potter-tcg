using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.GameActions.ActionParameters;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class DiscardAction : GameAction, IAbilityLoader
    {
        public Card Source { get; private set; }
        public int Amount { get; private set; }
        
        public List<Card> DiscardedCards { get; set; }

        public override string ToString()
        {
            return $"Discard Action - {Source.Data.CardName} sends {string.Join(", ", DiscardedCards.Select(c => c.Data.CardName))} to the discard pile.";
        }

        public void Load(IContainer game, Ability ability)
        {
            var parameter = DiscardActionParameter.FromString(ability.GetParams(nameof(DiscardAction)));

            Source = ability.Owner;
            Amount = parameter.Amount;
            DiscardedCards = ability.TargetSelector.SelectTargets(game);
        }
    }
}