using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class PlayerSystem : GameSystem, IAwake, IDestroy
    {
        private const int STARTING_HAND_AMOUNT = 7;
        
        private HandSystem _handSystem;
        private CreatureSystem _creatureSystem;
        private TurnSystem _turnSystem;

        public void Awake()
        {
            _handSystem = Container.GetSystem<HandSystem>();
            _creatureSystem = Container.GetSystem<CreatureSystem>();
            
            Global.Events.Subscribe(Notification.Perform<BeginGameAction>(), OnPerformBeginGame);
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Subscribe(Notification.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
        }

        private void OnPerformBeginGame(object sender, object args)
        {
            // IMPORTANT: Uncomment the shuffle deck reaction for preview builds.
            ShuffleDeck(Container.Match.LocalPlayer, Container.Match.EnemyPlayer);
            
            _handSystem.DrawCards(Container.Match.LocalPlayer, STARTING_HAND_AMOUNT);
            _handSystem.DrawCards(Container.Match.EnemyPlayer, STARTING_HAND_AMOUNT);

            Container.Match.CurrentPlayerIndex = 
                Random.Range(0f, 1f) < 0.5f
                    ? MatchData.LOCAL_PLAYER_INDEX
                    : MatchData.ENEMY_PLAYER_INDEX;
            
            Container.ChangeTurn();
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.Match.Players[action.NextPlayerIndex];
            _handSystem.DrawCards(player, 1);
            _creatureSystem.PerformCreatureDamagePhase(player);
        }

        private void OnPerformShuffleDeck(object sender, object args)
        {
            var action = (ShuffleDeckAction) args;

            foreach (var player in action.Targets)
            {
                player.Deck.Shuffle();
            }
        }

        public void ShuffleDeck(params Player[] players)
        {
            var action = new ShuffleDeckAction(players);
            
            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
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
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<BeginGameAction>(), OnPerformBeginGame);
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Unsubscribe(Notification.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
        }
    }
}