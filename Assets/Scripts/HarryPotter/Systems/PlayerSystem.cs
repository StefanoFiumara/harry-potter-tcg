using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
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
        private const int STARTING_HAND_AMOUNT = 7;
        
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Subscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
            Global.Events.Subscribe(Notification.Prepare<BeginGameAction>(), OnPrepareGameBegin);
            // TODO: Maybe introduce HandSystem for events related to playing cards from your hand - consider if PlayerSystem becomes too bloated.
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
        }

        private void OnPrepareGameBegin(object sender, object args)
        {
            DrawCards(Container.GameState.LocalPlayer, STARTING_HAND_AMOUNT);
            DrawCards(Container.GameState.EnemyPlayer, STARTING_HAND_AMOUNT);
        }
        
        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.GameState.Players[action.NextPlayerIndex];
            DrawCards(player, 1);
            DoCreatureDamagePhase(player);
        }

        private void OnPerformDrawCards(object sender, object args)
        {   
            var action = (DrawCardsAction) args;
 
            action.Cards = action.Player[Zones.Deck].Draw(action.Amount);
            foreach (var card in action.Cards)
            {
                ChangeZone(card, Zones.Hand);
            }
        }

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;
            
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

        //TODO: Can this phase be wrapped into its own action?
        private void DoCreatureDamagePhase(Player player)
        {
            foreach (var card in player.Creatures)
            {
                var creature = card.GetAttribute<Creature>();
                
                var damageAction = new DamageAction(card, Container.GameState.OppositePlayer, creature.Attack);
                Container.AddReaction(damageAction);
            }
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