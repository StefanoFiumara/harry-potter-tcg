using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;
using Utils;

namespace HarryPotter.Views
{
    public class PlayerView : MonoBehaviour
    {
        private const int STARTING_HAND_AMOUNT = 7;
        
        public Player Player;
        private GameViewSystem _gameView;
        private Dictionary<Zones, ZoneView> _zoneViews;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            Global.Events.Subscribe(Notification.Prepare<BeginGameAction>(), OnGameBegin);
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnDrawCards);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPlayCard);

            _zoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => z.Zone)
                .ToDictionary(g => g.Key, g => g.Single());
        }

        private void OnGameBegin(object sender, object args)
        {
            var playerSystem = _gameView.Container.GetSystem<PlayerSystem>();
            playerSystem.DrawCards(Player, STARTING_HAND_AMOUNT);
        }
        
        private void OnDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;
            if (action.Player.Index != Player.Index) return;
            
            action.PerformPhase.Viewer = DrawCardAnimation;
        }

        private void OnPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;
            if (action.Player.Index != Player.Index) return;

            action.PerformPhase.Viewer = PlayCardAnimation;
        }

        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;
            
            var fromZone = _zoneViews[Zones.Deck];
            var toZone = _zoneViews[Zones.Hand];
            
            var cardViews = fromZone.Cards.Where(view => drawAction.Cards.Contains(view.Card)).ToList();

            var anim = MoveToZoneAnimation(cardViews, fromZone, toZone);

            while (anim.MoveNext())
            {
                yield return null;
            }
        }

        private IEnumerator PlayCardAnimation(IContainer container, GameAction action)
        {
            var playAction = (PlayCardAction) action;
            
            var fromZone = _zoneViews[playAction.Card.Zone];
            var toZone = _zoneViews[playAction.Card.Data.Type.ToTargetZone()];


            var cardViews = fromZone.Cards.Where(view => view.Card == playAction.Card).ToList();

            var anim = MoveToZoneAnimation(cardViews, fromZone, toZone);
            
            while (anim.MoveNext())
            {
                yield return null;
            }
        }

        private IEnumerator MoveToZoneAnimation(List<CardView> cardViews, ZoneView fromZone, ZoneView toZone)
        {
            var sequence = DOTween.Sequence();
            foreach (var cardView in cardViews)
            {
                cardView.Card.Zone = Zones.Hand;
                fromZone.Cards.Remove(cardView);
                
                toZone.Cards.Add(cardView);

                var nextSequence = cardView.transform.Move(toZone.GetNextPosition(), toZone.GetTargetRotation());

                sequence = sequence.Append(nextSequence);
            }

            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }

        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<BeginGameAction>(), OnGameBegin);
            Global.Events.Unsubscribe(Notification.Prepare<DrawCardsAction>(), OnDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPlayCard);
        }
    }
}