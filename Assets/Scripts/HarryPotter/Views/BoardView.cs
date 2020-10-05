using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    //TODO: Split into multiple views if this starts doing too much
    public class BoardView : MonoBehaviour
    {
        private Dictionary<(int PlayerIndex, Zones Zone), ZoneView> ZoneViews { get; set; }

        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Subscribe(Notification.Prepare<DamageAction>(), OnPrepareDamage);
            Global.Events.Subscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            

            ZoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => (z.Owner.Index, z.Zone))
                .ToDictionary(g => g.Key, g => g.Single());
        }

        public ZoneView FindZoneView(Player player, Zones zone) => ZoneViews[(player.Index, zone)];
        private CardView FindCardView(Card card) => FindZoneView(card.Owner, card.Zone).Cards.Single(c => c.Card == card);
        public List<CardView> FindCardViews(List<Card> cards) => ZoneViews.Values.SelectMany(z => z.Cards).Where(cv => cards.Contains(cv.Card)).ToList();
        

        private void OnPrepareDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;
            action.PerformPhase.Viewer = DrawCardAnimation;
        }

        private void OnPreparePlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;
            action.PerformPhase.Viewer = PlayToBoardAnimation;
        }

        private void OnPrepareDamage(object sender, object args)
        {
            var action = (DamageAction) args;
            action.PerformPhase.Viewer = DamageAnimation;
        }

        private void OnPrepareCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            action.PerformPhase.Viewer  = CastSpellAnimation;
        }
        
        private void OnPrepareDiscard(object sender, object args)
        {
            var action = (DiscardAction) args;
            action.PerformPhase.Viewer  = DiscardAnimation;
        }

        private IEnumerator DiscardAnimation(IContainer container, GameAction action)
        {
            var discardAction = (DiscardAction) action;

            foreach (var card in discardAction.DiscardedCards)
            {
                var cardView = FindCardView(card);
                var anim = MoveToZoneAnimation(cardView, Zones.Discard);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator CastSpellAnimation(IContainer container, GameAction action)
        {
            // TODO: Spell Preview Animation 
            var castSpellAction = (CastSpellAction) action;
            var cardView = FindCardView(castSpellAction.Card);
            var anim = MoveToZoneAnimation(cardView, Zones.Discard);
            while (anim.MoveNext())
            {
                yield return null;
            }
        }

        private IEnumerator DamageAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var damageAction = (DamageAction) action;

            var sourceView = FindCardView(damageAction.Source);
            var cardViews = FindCardViews(damageAction.DiscardedCards);

            sourceView.Highlight(Colors.DoingEffect); //TODO: This might go away outside of the CreatureDamagePhase
            
            //NOTE: Animating through this list backwards animates the cards in the right order when there's more than one card to take from the pile.
            for (var i = cardViews.Count - 1; i >= 0; i--)
            {
                var cardView = cardViews[i];
                var anim = MoveToZoneAnimation(cardView, Zones.Discard);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
            
            sourceView.Highlight(Color.clear);
        }

        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;

            var cardViews = FindCardViews(drawAction.DrawnCards);
            
            //NOTE: Animating through this list backwards animates the cards in the right order when there's more than one card to take from the pile.
            for (var i = cardViews.Count - 1; i >= 0; i--)
            {
                var cardView = cardViews[i];
                var anim = MoveToZoneAnimation(cardView, Zones.Hand, Zones.Deck);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator PlayToBoardAnimation(IContainer container, GameAction action)
        {
            var playAction = (PlayToBoardAction) action;

            var cardViewPairs = FindCardViews(playAction.Cards)
                .Select(view => (view, view.Card.Data.Type.ToTargetZone()))
                .ToList();
            
            var anim = MoveToZoneAnimation(cardViewPairs);
            while (anim.MoveNext())
            {
                yield return anim.Current;
            }
        }

        /// <summary>
        /// Moves a card to the specified zone
        /// </summary>
        /// <param name="cardView">the card to move</param>
        /// <param name="to">the zone to move it to</param>
        /// <param name="from">Optional - if provided, will use this zone as the "from", needed if the card's Zone property has already been updated to the new one.</param>
        /// <returns></returns>
        private IEnumerator MoveToZoneAnimation(CardView cardView, Zones to, Zones from = Zones.None)
        {
            var pairs = new List<(CardView, Zones)>
            {
                (cardView, to)
            };
            
            return MoveToZoneAnimation(pairs, from);
        }
        
        private IEnumerator MoveToZoneAnimation(List<(CardView, Zones)> cardViewPairs, Zones from = Zones.None)
        {
            var affectedZones = new HashSet<ZoneView>();

            foreach (var pair in cardViewPairs)
            {
                var (view, zone) = pair;
                
                var toZone = FindZoneView(view.Card.Owner, zone);
                var fromZone = FindZoneView(view.Card.Owner, from != Zones.None ? from : view.Card.Zone);

                fromZone.Cards.Remove(view);
                toZone.Cards.Add(view);
                view.transform.SetParent(toZone.transform);

                affectedZones.Add(toZone);
                affectedZones.Add(fromZone);
            }
            
            var sequence = DOTween.Sequence();
            
            foreach (var zoneView in affectedZones)
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
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Unsubscribe(Notification.Prepare<DamageAction>(), OnPrepareDamage);
            Global.Events.Unsubscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
        }
    }
}