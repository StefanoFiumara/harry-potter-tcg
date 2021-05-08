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
        private MatchData _match;

        private GameSettings _settings;

        public void Awake()
        {
            _handSystem = Container.GetSystem<HandSystem>();
            _creatureSystem = Container.GetSystem<CreatureSystem>();
            _match = Container.GetMatch();
            _settings = Container.GetSystem<PlayerSettingsSystem>().Settings;
            
            Global.Events.Subscribe(Notification.Perform<BeginGameAction>(), OnPerformBeginGame);
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Subscribe(Notification.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
        }

        private void OnPerformBeginGame(object sender, object args)
        {

            if (!_settings.DebugMode)
            {
                ShuffleDeck(_match.LocalPlayer, _match.EnemyPlayer);
            }
            
            _handSystem.DrawCards(_match.LocalPlayer, STARTING_HAND_AMOUNT);
            _handSystem.DrawCards(_match.EnemyPlayer, STARTING_HAND_AMOUNT);

            _match.CurrentPlayerIndex = 
                Random.Range(0f, 1f) < 0.5f
                    ? MatchData.LOCAL_PLAYER_INDEX
                    : MatchData.ENEMY_PLAYER_INDEX;
            
            Container.ChangeTurn();
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = _match.Players[action.NextPlayerIndex];
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

        /// <summary>
        /// Shuffles the deck of the given players
        /// </summary>
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
        
        /// <summary>
        /// Changes the zone of a Card and adds it to the correct stack in the Player's data. 
        /// </summary>
        /// <param name="card">The card to update.</param>
        /// <param name="zone">The card's destination zone.</param>
        /// <param name="toPlayer">Optional - change a card's ownership to this player</param>
        /// <param name="placeAtBottom">Optional - true will place the card at the bottom of the destination's stack instead of the top</param>
        public void ChangeZone(Card card, Zones zone, Player toPlayer = null, bool placeAtBottom = false)
        {
            var fromPlayer = card.Owner;
            toPlayer = toPlayer != null ? toPlayer : fromPlayer;

            if (card.Zone != Zones.None)
            {
                fromPlayer[card.Zone].Remove(card);    
            }

            if (zone != Zones.None)
            {
                if (placeAtBottom)
                {
                    toPlayer[zone]?.Insert(0, card);
                }
                else
                {
                    toPlayer[zone]?.Add(card);
                }
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