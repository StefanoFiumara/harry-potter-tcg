using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    //NOTE: Split into multiple views if this starts doing too much
    public class BoardView : MonoBehaviour
    {
        private static readonly Vector3 SpellPreviewPos = new Vector3(0f, 0f, 40f);
        private static readonly Vector3 SpellPreviewRot = new Vector3(0f, 180f, 0f);
        
        private GameViewSystem _gameView;

        private Dictionary<(int PlayerIndex, Zones Zone), ZoneView> ZoneViews { get; set; }

        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamage);
            Global.Events.Subscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            
            ZoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => (z.Owner.Index, z.Zone))
                .ToDictionary(g => g.Key, g => g.Single());
            
            _gameView = GetComponentInParent<GameViewSystem>();

            if (_gameView == null)
            {
                Debug.LogError("BoardView could not find GameView");
            }
        }

        public ZoneView FindZoneView(Player player, Zones zone) => ZoneViews[(player.Index, zone)];
        private CardView FindCardView(Card card) => FindCardViews(new List<Card> { card }).Single();
        public List<CardView> FindCardViews(List<Card> cards) => ZoneViews.Values.SelectMany(z => z.Cards).Where(cv => cards.Contains(cv.Card)).ToList();
        
        private void OnPreparePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.Card.Data.Type == CardType.Spell)
            {
                action.PerformPhase.Viewer = SpellPreviewAnimation;
            }
        }
        
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
            var action = (DamagePlayerAction) args;
            action.PerformPhase.Viewer = DamagePlayerAnimation;
        }

        private void OnPrepareDiscard(object sender, object args)
        {
            var action = (DiscardAction) args;
            action.PerformPhase.Viewer  = DiscardAnimation;
        }

        private IEnumerator SpellPreviewAnimation(IContainer container, GameAction action)
        {
            var playCardAction = (PlayCardAction) action;
            var cardView = FindCardView(playCardAction.Card);

            var handZone = FindZoneView(playCardAction.Player, Zones.Hand);
            var discardZone = FindZoneView(playCardAction.Player, Zones.Discard);
            
            var targetPos = discardZone.GetNextPosition();
            var targetRot = discardZone.GetRotation();
            
            ChangeZoneView(cardView, Zones.Discard, from: Zones.Hand);
            yield return true; //NOTE: Moves the card out of the Hand Zone
            
            var sequence = DOTween.Sequence()
                    .Append(cardView.Move(SpellPreviewPos, SpellPreviewRot))
                    .Join(handZone.DoZoneLayoutAnimation())
                    .AppendInterval(0.75f)
                    .Append(cardView.Move(targetPos, targetRot))
                    .AppendInterval(0.25f);

            yield return null; // TODO: Test if this can be removed
            
            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }

        private IEnumerator DamagePlayerAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var damageAction = (DamagePlayerAction) action;

            if (damageAction.Source.Data.Type.HasCardType(CardType.Spell | CardType.Creature))
            {
                if (damageAction.Source.Data.Type == CardType.Spell)
                {
                    var target = damageAction.Target[Zones.Characters].First(); // NOTE: Should always be the starting character.
                    var particleType = damageAction.Source.GetAttribute<LessonCost>().Type;
                    var particleSequence = GetParticleSequence(damageAction.Player, target, particleType);

                    while (particleSequence.IsPlaying())
                    {
                        yield return null;
                    }                    
                }

                if (damageAction.Source.Data.Type == CardType.Creature)
                {
                    var target = damageAction.Target[Zones.Characters].First(); // NOTE: Should always be the starting character.
                    var particleSequence = GetParticleSequence(damageAction.Source, target);

                    while (particleSequence.IsPlaying())
                    {
                        yield return null;
                    }    
                }
                
            }
            
            var discardedCards = FindCardViews(damageAction.DiscardedCards);
            
            for (var i = discardedCards.Count - 1; i >= 0; i--)
            {
                var cardView = discardedCards[i];
                var anim = MoveToZoneAnimation(cardView, Zones.Discard, Zones.Deck);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private Sequence GetParticleSequence(Player source, Card target, LessonType particleColorType)
        {
            var targetView = FindCardView(target);

            // TODO: Constants
            var startPosLocal = new Vector3(0f, -18.5f, 50f); // For targeting enemy
            var startPosEnemy = new Vector3(0f, 21.5f, 50f); // For targeting local

            var startPos = source == _gameView.Match.LocalPlayer
                ? startPosLocal
                : startPosEnemy;

            var targetPos = targetView.transform.position + 0.5f * Vector3.back;

            _gameView.ParticleSystem.SetParticleColorGradient(particleColorType);
            
            // TODO: Consolidate the parameters involved in this sequence so that they are not repeated across overloads
            return DOTween.Sequence()
                .AppendCallback(() => _gameView.ParticleSystem.Play())
                .Append(_gameView.ParticleSystem.transform.DOMove(startPos, 0f))
                .Append(_gameView.ParticleSystem.transform.DOMove(targetPos, 1.25f).SetEase(Ease.OutQuint))
                .AppendCallback(() => _gameView.ParticleSystem.Stop());
        }
        
        private Sequence GetParticleSequence(Card source, Card target)
        {
            var sourceView = FindCardView(source);
            var targetView = FindCardView(target);

            var startPos = sourceView.transform.position + 0.5f * Vector3.back;
            var targetPos = targetView.transform.position + 0.5f * Vector3.back;

            var particleColorType = sourceView.Card.GetAttribute<LessonCost>().Type;
            _gameView.ParticleSystem.SetParticleColorGradient(particleColorType);

            return DOTween.Sequence()
                .AppendCallback(() => _gameView.ParticleSystem.Play())
                .Append(_gameView.ParticleSystem.transform.DOMove(startPos, 0f))
                .Append(_gameView.ParticleSystem.transform.DOMove(targetPos, 1.25f).SetEase(Ease.OutQuint))
                .AppendCallback(() => _gameView.ParticleSystem.Stop());
        }

        private IEnumerator DiscardAnimation(IContainer container, GameAction action)
        {
            var discardAction = (DiscardAction) action;

            if (discardAction.Source.Data.Type == CardType.Spell)
            {
                var sequence = DOTween.Sequence();

                foreach (var discardedCard in discardAction.DiscardedCards)
                {
                    // NOTE: This check needs to be done so that spell cards that discard themselves don't do the particle animation 
                    if (discardAction.Source == discardedCard)
                    {
                        continue;
                    }

                    var particleType = discardAction.Source.GetAttribute<LessonCost>().Type;
                    var particleSequence = GetParticleSequence(discardAction.Player, discardedCard, particleType);
                    sequence.Append(particleSequence);
                }
                
                while (sequence.IsPlaying())
                {
                    yield return null;
                }
            }
            
            var cardViews = FindCardViews(discardAction.DiscardedCards);

            foreach (var cardView in cardViews)
            {
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
            

            foreach (var (card, zone) in cardViewPairs)
            {
                if (zone == Zones.None)
                {
                    yield break;
                }
                
                var affected = ChangeZoneView(card, zone, from);

                foreach (var zoneView in affected)
                {
                    affectedZones.Add(zoneView);
                }
            }
            
            // TODO: We might not want to rely on DoZoneLayoutAnimation to move cards between zones.
            //       It makes it difficult to do more custom animations from one zone to the other.
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
        
        private List<ZoneView> ChangeZoneView(CardView card, Zones to, Zones from = Zones.None)
        {
            var result = new List<ZoneView>();
            var fromTemp = from != Zones.None ? from : card.Card.Zone;

            if (fromTemp != Zones.None)
            {
                var fromZone = FindZoneView(card.Card.Owner, fromTemp);
                fromZone.Cards.Remove(card);
                result.Add(fromZone);
            }
            
            var toZone = FindZoneView(card.Card.Owner, to);
            toZone.Cards.Add(card);
            result.Add(toZone);
            
            card.transform.SetParent(toZone.transform);
            return result;
        }

        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamage);
            Global.Events.Unsubscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
        }
    }
}