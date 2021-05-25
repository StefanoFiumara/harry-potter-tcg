using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class HandSystem : GameSystem, IAwake, IDestroy
    {
        private PlayerSystem _playerSystem;

        public void Awake()
        {
            _playerSystem = Container.GetSystem<PlayerSystem>();
            Global.Events.Subscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
            Global.Events.Subscribe(Notification.Perform<ReturnToHandAction>(), OnPerformReturnToHand);
        }

        private void OnPerformReturnToHand(object sender, object args)
        {
            var action = (ReturnToHandAction) args;

            foreach (var card in action.ReturnedCards)
            {
                _playerSystem.ChangeZone(card, Zones.Hand);
            }
        }

        private void OnPerformDrawCards(object sender, object args)
        {   
            var action = (DrawCardsAction) args;
 
            action.DrawnCards = action.Player[Zones.Deck].Draw(action.Amount);
            foreach (var card in action.DrawnCards) // NOTE: No need to reserve loop since order doesn't matter here?
            {
                _playerSystem.ChangeZone(card, Zones.Hand);
            }
        }

        public void DrawCards(Player player, int amount, bool usePlayerAction = false)
        {
            var action = new DrawCardsAction(player, amount, usePlayerAction);
            
            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
            }
        }

        public void ReturnToHand(Card source, Card target)
        {
            var action = new ReturnToHandAction
            {
                SourceCard = source,
                Player = source.Owner,
                ReturnedCards = new List<Card> { target }
            };

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
            Global.Events.Unsubscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
            Global.Events.Unsubscribe(Notification.Perform<ReturnToHandAction>(), OnPerformReturnToHand);
        }
    }
}