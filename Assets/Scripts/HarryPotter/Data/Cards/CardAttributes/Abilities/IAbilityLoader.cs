using HarryPotter.Systems.Core;

namespace HarryPotter.Data.Cards.CardAttributes.Abilities
{
    public interface IAbilityLoader
    {
        void Load(IContainer game, Ability ability);
    }
}