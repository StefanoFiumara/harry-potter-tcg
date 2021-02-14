using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class HealingAction : GameAction, IAbilityLoader
    {
        public Card Source { get; set; }
        
        public List<Card> HealedCards { get; set; }
        

        // TODO: Verify if this specific ctor is needed, for now I think most healing cards will use ability loader
        public HealingAction(Card source, List<Card> targets)
        {
            Source = source;
            HealedCards = targets;
        }

        public HealingAction()
        {
            
        }

        public void Load(IContainer game, Ability ability)
        {
            Source = ability.Owner;
        
            Player = Source.Owner;
            HealedCards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }
    }
}