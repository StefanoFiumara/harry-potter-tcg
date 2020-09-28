using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class CardSystem : GameSystem , IAwake//, IDestroy
    {
        private TargetSystem _targetSystem;
        
        public List<Card> PlayableCards { get; set; } = new List<Card>();

        public void Awake()
        {
            _targetSystem = Container.GetSystem<TargetSystem>();
        }
        
        public void Refresh(ControlMode mode)
        {
            PlayableCards.Clear();

            foreach (var card in Container.Match.CurrentPlayer[Zones.Hand])
            {
                _targetSystem.AutoTarget(card, mode);
                
                var playAction = new PlayCardAction(card);
                if (playAction.Validate().IsValid)
                {
                    PlayableCards.Add(card);
                }
            }
        }

        public bool IsPlayable(Card card) => PlayableCards.Contains(card);
        
    }
}