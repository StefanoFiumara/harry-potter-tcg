using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using UnityEngine;
using Utils;

namespace HarryPotter.Systems
{
    public class PlayerSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Subscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);

            // TODO: I think these events belong in HandSystem
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.GameState.Players[action.NextPlayerIndex];
            DrawCards(player, 1);
            //TODO: Creature Damage Phase goes here
        }

        private void OnPerformDrawCards(object sender, object args)
        {   
            var action = (DrawCardsAction) args;

            if (action.UsePlayerAction)
            {
                action.Player.ActionsAvailable--;
            }
            
            action.Cards = action.Player[Zones.Deck].Draw(action.Amount);
            foreach (var card in action.Cards)
            {
                ChangeZone(card, Zones.Hand);
            }
        }

        private void OnPreparePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            //TODO: Separate into ManaSystem (PlayerActionSystem?), Implement Validate step for Game Actions
            if (action.UsePlayerAction && action.Player.ActionsAvailable == 0)
            {
                action.Cancel();
            }
        }
        
        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.UsePlayerAction)
            {
                action.Player.ActionsAvailable--;
            }
            
            ChangeZone(action.Card, action.Card.Data.Type.ToTargetZone());
        }

        public void DrawCards(Player player, int amount, bool usePlayerAction = false)
        {
            var action = new DrawCardsAction(player, amount, usePlayerAction);
            if(Container.GetSystem<ActionSystem>().IsActive)
                Container.AddReaction(action);
            else 
                Container.Perform(action);
        }

        public void ChangeZone(Card card, Zones zone, Player toPlayer = null)
        {
            var fromPlayer = card.Owner;
            toPlayer = toPlayer ? toPlayer : fromPlayer;
            fromPlayer[card.Zone].Remove(card);
            toPlayer[zone].Add(card);
            card.Zone = zone;
            card.Owner = toPlayer;

        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Unsubscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
        }
    }
}