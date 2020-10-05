using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class PlayToBoardAction : GameAction, IAbilityLoader
    {
        public List<Card> Cards { get; private set; }

        public PlayToBoardAction(Card card)
        {
            Cards = new List<Card> { card };
            Player = card.Owner;
        }

        public PlayToBoardAction()
        {
            
        }

        public override string ToString()
        {
            return $"PlayToBoardAction - Player {Player.Index} plays {string.Join(", ", Cards.Select(c => c.Data.CardName))} to Board";
        }

        public void Load(IContainer game, Ability ability)
        {
            Player = ability.Owner.Owner;
            Cards = ability.TargetSelector.SelectTargets(game);
        }
    }
}