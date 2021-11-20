using HarryPotter.Data;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class CreatureDamagePhaseAction : GameAction, IAbilityLoader
    {
        public CreatureDamagePhaseAction()
        {

        }

        public CreatureDamagePhaseAction(Player player)
        {
            Player = player;
        }

        public void Load(IContainer game, Ability ability)
        {
            SourceCard = ability.Owner;
            Player = SourceCard.Owner;
        }
    }
}
