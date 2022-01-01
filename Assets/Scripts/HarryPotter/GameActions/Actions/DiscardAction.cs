using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class DiscardAction : GameAction, IAbilityLoader
    {
        public List<Card> DiscardedCards { get; set; }

        public GameAction SourceAction { get; set; }

        public void Load(IContainer game, Ability ability)
        {
            SourceCard = ability.Owner;

            Player = SourceCard.Owner;
            DiscardedCards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }

        public override string ToString()
        {
            return DiscardedCards.Count == 0
                ? string.Empty
                : $"Discard Action - {SourceCard.Data.CardName} sends {string.Join(", ", DiscardedCards.Select(c => c.Data.CardName))} to the discard pile.";
        }
    }
}
