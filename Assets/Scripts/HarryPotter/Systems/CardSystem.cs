using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class CardSystem : GameSystem , IAwake//, IDestroy
    {
        private TargetSystem _targetSystem;
        
        public List<Card> PlayableCards { get; set; } = new List<Card>();
        public List<Card> ActivatableCards { get; set; } = new List<Card>();

        public void Awake()
        {
            _targetSystem = Container.GetSystem<TargetSystem>();
        }
        
        public void Refresh(ControlMode mode)
        {
            PlayableCards.Clear();
            ActivatableCards.Clear();

            foreach (var card in Container.Match.CurrentPlayer[Zones.Hand])
            {
                _targetSystem.AutoTarget(card, AbilityType.PlayEffect, mode);
                _targetSystem.AutoTarget(card, AbilityType.PlayCondition, mode);
                
                var playAction = new PlayCardAction(card);
                if (playAction.Validate().IsValid)
                {
                    PlayableCards.Add(card);
                }
            }

            foreach (var card in Container.Match.CurrentPlayer.CardsInPlay)
            {
                _targetSystem.AutoTarget(card, AbilityType.ActivateCondition, mode);
                _targetSystem.AutoTarget(card, AbilityType.ActivateEffect, mode);
                
                var activateAction = new ActivateCardAction(card);
                if (activateAction.Validate().IsValid)
                {
                    ActivatableCards.Add(card);
                }
            }
        }

        public bool IsPlayable(Card card) => PlayableCards.Contains(card);
        public bool IsActivatable(Card card) => ActivatableCards.Contains(card);
        
    }
}