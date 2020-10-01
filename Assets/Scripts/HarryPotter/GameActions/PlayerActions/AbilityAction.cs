using HarryPotter.Data.Cards.CardAttributes.Abilities;

namespace HarryPotter.GameActions.PlayerActions
{
    public class AbilityAction : GameAction
    {
        public Ability Ability { get; private set; }

        public AbilityAction(Ability ability)
        {
            Ability = ability;
        }
    }
}