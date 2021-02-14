using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class HealingSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<HealingAction>(), OnPerformHealingAction);
        }

        private void OnPerformHealingAction(object sender, object args)
        {
            var action = (HealingAction) args;

            var playerSystem = Container.GetSystem<PlayerSystem>();

            foreach (var card in action.HealedCards)
            {
                playerSystem.ChangeZone(card, Zones.Deck, placeAtBottom: true);
            }
        }

        public void HealPlayer(Card source, List<Card> targets)
        {
            var action = new HealingAction(source, targets);
            
            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<HealingAction>(), OnPerformHealingAction);
        }
    }
}