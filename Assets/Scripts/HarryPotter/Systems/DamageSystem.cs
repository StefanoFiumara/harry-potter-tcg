using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using Utils;

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
    public class DamageSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<DamageAction>(), OnPerformDamage);
        }

        private void OnPerformDamage(object sender, object args)
        {
            var action = (DamageAction) args;

            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            action.Cards = action.Target[Zones.Deck].Draw(action.Amount);
            foreach (var card in action.Cards)
            {
                playerSystem.ChangeZone(card, Zones.Discard);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<DamageAction>(), OnPerformDamage);            
        }
    }
}