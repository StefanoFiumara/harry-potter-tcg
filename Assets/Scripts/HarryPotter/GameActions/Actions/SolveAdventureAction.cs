using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class SolveAdventureAction : GameAction, IAbilityLoader
    {
        public Card Target { get; private set; }

        public SolveAdventureAction() { }
        public SolveAdventureAction(Card target, Player source)
        {
            SourceCard = target;
            Target = target;
            Player = source;
        }

        public override string ToString()
        {
            return $"SolveAdventureAction - {Player.PlayerName} solves the adventure {Target.Data.CardName}.";
        }

        public void Load(IContainer game, Ability ability)
        {
            SourceCard = ability.Owner;
            Player = SourceCard.Owner;
            Target = ability.TargetSelector.SelectTargets(game, ability.Owner).First();
        }
    }
}
