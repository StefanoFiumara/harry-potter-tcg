using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class HealingAction : GameAction, IAbilityLoader
    {
        public List<Card> HealedCards { get; set; }
        
        public HealingAction() { }
        public HealingAction(Card source, List<Card> targets)
        {
            SourceCard = source;
            HealedCards = targets;
        }
        
        public void Load(IContainer game, Ability ability)
        {
            SourceCard = ability.Owner;
        
            Player = SourceCard.Owner;
            HealedCards = ability.TargetSelector.SelectTargets(game, ability.Owner);
        }

        public override string ToString()
        {
            return HealedCards.Count == 0 
                ? string.Empty 
                : $"Healing Action - {SourceCard.Data.CardName} heals {HealedCards.Count} for {Player.PlayerName}.";            
        }
    }
}