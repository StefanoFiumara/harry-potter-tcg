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
    //TODO: Split into multiple views (?)
    public class PlayerView : MonoBehaviour
    {
        public Player Player;
        private GameViewSystem _gameView;
        private Dictionary<Zones, ZoneView> _zoneViews;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            Global.Events.Subscribe(Notification.Prepare<DamageAction>(), OnPrepareDamage);

            _zoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => z.Zone)
                .ToDictionary(g => g.Key, g => g.Single());
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

        private void OnPrepareDamage(object sender, object args)
        {
            var action = (DamageAction) args;
            if (action.Target.Index != Player.Index) return;

            action.PerformPhase.Viewer = DamageAnimation;
        }

        private IEnumerator DamageAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var damageAction = (DamageAction) action;

            var fromZone = _zoneViews[Zones.Deck];

            var cardViews = fromZone.Cards.Where(view => damageAction.Cards.Contains(view.Card)).ToList();

            //NOTE: Animating through this list backwards animates the cards correct when there's more than one card to take from the deck.
            for (var i = cardViews.Count - 1; i >= 0; i--)
            {
                var cardView = cardViews[i];
                var anim = MoveToZoneAnimation(cardView, Zones.Discard);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }
    
        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;
            
            var fromZone = _zoneViews[Zones.Deck];
            
            var cardViews = fromZone.Cards.Where(view => drawAction.Cards.Contains(view.Card)).ToList();

            //NOTE: Animating through this list backwards animates the cards correct when there's more than one card to take from the deck.
            for (var i = cardViews.Count - 1; i >= 0; i--)
            {
                var cardView = cardViews[i];
                var anim = MoveToZoneAnimation(cardView, Zones.Hand);
                while (anim.MoveNext())
                {
                    yield return null;
                }
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
                yield return anim.Current;
            }
        }

        private IEnumerator MoveToZoneAnimation(CardView cardView, Zones zone)
        {
            return MoveToZoneAnimation(new List<CardView> {cardView}, zone);
        }
        
        private IEnumerator MoveToZoneAnimation(List<CardView> cardViews, Zones zone)
        {
            var toZone = _zoneViews[zone];
            var affectedZones = cardViews.Select(v => v.Card.Zone).ToHashSet();
            affectedZones.Add(zone);

            foreach (var cardView in cardViews)
            {
                var fromZone = _zoneViews[cardView.Card.Zone];

                fromZone.Cards.Remove(cardView);
                toZone.Cards.Add(cardView);
                cardView.transform.SetParent(toZone.transform);
            }
            
            var affectedZoneViews = _zoneViews.WhereIn(affectedZones);
            
            var sequence = DOTween.Sequence();
            
            foreach (var zoneView in affectedZoneViews)
            {
                sequence = sequence.Join(zoneView.DoZoneLayoutAnimation());
            }

            yield return true;
            
            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }

        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            Global.Events.Unsubscribe(Notification.Prepare<DamageAction>(), OnPrepareDamage);
        }
    }
}