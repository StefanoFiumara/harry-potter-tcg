using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class DummyAction : GameAction, IAbilityLoader
    {
        public void Load(IContainer game, Ability ability)
        {
            // Do nothing
        }

        public override string ToString() => string.Empty;
    }
}