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
        
        public HealingAction() { }
        public HealingAction(Card source, List<Card> targets)
        {
            Source = source;
            HealedCards = targets;
        }
        
        public void Load(IContainer game, Ability ability)
        {
            Source = ability.Owner;
        
            Player = Source.Owner;
            HealedCards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }

        public override string ToString()
        {
            return HealedCards.Count == 0 
                ? string.Empty 
                : $"Healing Action - {Source.Data.CardName} heals {HealedCards.Count} for {Player.PlayerName}.";            
        }
    }
}