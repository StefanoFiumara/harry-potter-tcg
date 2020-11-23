using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.GameActions.ActionParameters;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class DamagePlayerOrCreatureAction : GameAction, IAbilityLoader
    {
        public Card Source { get; private set; }
        public Card Target { get; private set; }
        public int Amount { get; private set; }
        
        public DamagePlayerOrCreatureAction(Card source, Card target, int amount)
        {
            Source = source;
            Target = target;
            Amount = amount;
        }

        public DamagePlayerOrCreatureAction()
        {
            
        }
        
        public void Load(IContainer game, Ability ability)
        {
            var parameter = DamagePlayerOrCreatureParameter.FromString(ability.GetParams(nameof(DamagePlayerOrCreatureAction)));

            Amount = parameter.Amount;
            Source = ability.Owner;
            Player = Source.Owner;

            Target = ability.TargetSelector.SelectTargets(game, ability.Owner).Single();
        }
    }
}