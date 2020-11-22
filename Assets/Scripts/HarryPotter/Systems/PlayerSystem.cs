using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

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
        }

        private void OnPrepareGameBegin(object sender, object args)
        {
            DrawCards(Container.Match.LocalPlayer, STARTING_HAND_AMOUNT);
            DrawCards(Container.Match.EnemyPlayer, STARTING_HAND_AMOUNT);
        }
        
        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.Match.Players[action.NextPlayerIndex];
            DrawCards(player, 1);
            DoCreatureDamagePhase(player);
        }

        private void OnPerformDrawCards(object sender, object args)
        {   
            var action = (DrawCardsAction) args;
 
            action.DrawnCards = action.Player[Zones.Deck].Draw(action.Amount);
            foreach (var card in action.DrawnCards)
            {
                ChangeZone(card, Zones.Hand);
            }
        }

        public void ChangeZone(Card card, Zones zone, Player toPlayer = null)
        {
            var fromPlayer = card.Owner;
            toPlayer = toPlayer != null ? toPlayer : fromPlayer;

            if (card.Zone != Zones.None)
            {
                fromPlayer[card.Zone].Remove(card);    
            }

            if (zone != Zones.None)
            {
                toPlayer[zone]?.Add(card);
            }
            
            card.Zone = zone;
            card.Owner = toPlayer;
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
                
                var damageAction = new DamageAction(card, Container.Match.OppositePlayer, creature.Attack);
                Container.AddReaction(damageAction);
            }
        }
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Unsubscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<BeginGameAction>(), OnPrepareGameBegin);
        }
    }
}