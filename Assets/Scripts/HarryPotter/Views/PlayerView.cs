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
            Global.Events.Subscribe(Notification.Prepare<BeginGameAction>(), OnPrepareGameBegin);
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);

            _zoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => z.Zone)
                .ToDictionary(g => g.Key, g => g.Single());
        }

        private void OnPrepareGameBegin(object sender, object args)
        {
            var playerSystem = _gameView.Container.GetSystem<PlayerSystem>();
            playerSystem.DrawCards(Player, STARTING_HAND_AMOUNT);
        }
        
        private void OnPrepareDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;
            if (action.Player.Index != Player.Index) return;
            
            action.PerformPhase.Viewer = DrawCardAnimation;
        }

        private void OnPreparePlayCard(object sender, object args)
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
            
            var cardViews = fromZone.Cards.Where(view => drawAction.Cards.Contains(view.Card)).ToList();

            var anim = MoveToZoneAnimation(cardViews, Zones.Hand);

            while (anim.MoveNext())
            {
                yield return null;
            }
        }

        private IEnumerator PlayCardAnimation(IContainer container, GameAction action)
        {
            var playAction = (PlayCardAction) action;
            
            var fromZone = _zoneViews[playAction.Card.Zone];

            var cardViews = fromZone.Cards.Where(view => view.Card == playAction.Card).ToList();

            var anim = MoveToZoneAnimation(cardViews, playAction.Card.Data.Type.ToTargetZone());
            
            while (anim.MoveNext())
            {
                yield return null;
            }
        }

        private IEnumerator MoveToZoneAnimation(List<CardView> cardViews, Zones zone, bool simultaneous = false)
        {
            var toZone = _zoneViews[zone];
            
            var sequence = DOTween.Sequence();
            foreach (var cardView in cardViews)
            {
                var fromZone = _zoneViews[cardView.Card.Zone];
                cardView.Card.Zone = Zones.Hand;
                
                fromZone.Cards.Remove(cardView);
                
                var nextSequence = cardView.transform.Move(toZone.GetNextPosition(), toZone.GetTargetRotation());
                
                toZone.Cards.Add(cardView);
                cardView.transform.SetParent(toZone.transform);
                
                sequence = simultaneous ? sequence.Join(nextSequence) : sequence.Append(nextSequence);
                sequence.Join(fromZone.ZoneLayoutAnimation());
            }

            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }

        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<BeginGameAction>(), OnPrepareGameBegin);
            Global.Events.Unsubscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
        }
    }
}