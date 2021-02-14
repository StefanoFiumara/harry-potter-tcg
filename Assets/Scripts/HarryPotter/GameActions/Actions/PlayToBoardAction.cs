using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class PlayToBoardAction : GameAction, IAbilityLoader
    {
        public List<Card> Cards { get; private set; }

        public PlayToBoardAction() { }
        public PlayToBoardAction(Card card)
        {
            Cards = new List<Card> { card };
            Player = card.Owner;
        }
        
        public void Load(IContainer game, Ability ability)
        {
            Player = ability.Owner.Owner;
            Cards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }

        public override string ToString()
        {
            return $"PlayToBoardAction - {Player.PlayerName} plays {string.Join(", ", Cards.Select(c => c.Data.CardName))} to the Board.";
        }
    }
}