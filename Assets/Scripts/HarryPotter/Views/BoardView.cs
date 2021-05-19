using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Input.InputStates;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    // TODO: Split into multiple views when this file gets too big, maybe some of the animations can be handled elsewhere.
    public class BoardView : MonoBehaviour
    {
        // TODO: Should we store these positions via some transform in the hierarchy? What's more maintainable?
        private static readonly Vector3 RevealPosition = new Vector3(0f, 0f, 40f);
        private static readonly Vector3 RevealRotation = new Vector3(0f, 180f, 0f);
        
        private GameView _gameView;

        // TODO: Same as PilePreviewPosition from ZoneView - Maybe split into HealingView to clean this up
        private static readonly Vector3 HealingPreviewPosition = new Vector3
        {
            x = -27.5f,
            y = 0f,
            z = 79f
        };
        
        private static readonly Vector2 HealingPreviewSpacing = new Vector2
        {
            x = 1.1f,
            y = 0.2f
        };

        private static readonly int HealingPreviewColumnCount = 5;
        
        private void Awake()
        {
            
            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            
            Global.Events.Subscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamagePlayer);
            Global.Events.Subscribe(Notification.Prepare<DamageCreatureAction>(), OnPrepareDamageCreature);
            Global.Events.Subscribe(Notification.Prepare<HealingAction>(), OnPrepareHealing);
            
            Global.Events.Subscribe(Notification.Prepare<ShuffleDeckAction>(), OnPrepareShuffleDeck);
            
            _gameView = GetComponent<GameView>();

            if (_gameView == null)
            {
                Debug.LogError("BoardView could not find GameView");
            }
        }

        private void OnPreparePlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;
            action.PerformPhase.Viewer = PlayToBoardAnimation;
        }

        private void OnPrepareDiscard(object sender, object args)
        {
            var action = (DiscardAction) args;
            action.PerformPhase.Viewer  = DiscardAnimation;
        }

        private void OnPrepareDamagePlayer(object sender, object args)
        {
            var action = (DamagePlayerAction) args;
            action.PerformPhase.Viewer = DamagePlayerAnimation;
        }

        private void OnPrepareDamageCreature(object sender, object args)
        {
            var action = (DamageCreatureAction) args;
            action.PerformPhase.Viewer  = DamageCreatureAnimation;
        }

        private void OnPrepareHealing(object sender, object args)
        {
            var action = (HealingAction) args;
            action.PerformPhase.Viewer = HealingAnimation;
        }

        private void OnPrepareShuffleDeck(object sender, object args)
        {
            var action = (ShuffleDeckAction) args;
            action.PerformPhase.Viewer  = ShuffleDeckAnimation;
        }

        public Sequence GetRevealSequence(CardView target, Zones to, Zones from, float duration = 0.5f)
        {
            var endZoneView = _gameView.FindZoneView(target.Card.Owner, to);

            var previewSequence = DOTween.Sequence()
                .Append(target.Move(RevealPosition, RevealRotation, duration));

            _gameView.ChangeZoneView(target, to, from);
            
            if (from != Zones.None)
            {
                var startZoneView = _gameView.FindZoneView(target.Card.Owner, from);
                previewSequence.Join(startZoneView.GetZoneLayoutSequence(duration));
            }

            var finalPos = endZoneView.GetNextPosition();
            var finalRot = endZoneView.GetRotation();
            
            return previewSequence
                .AppendInterval(duration)
                .Append(target.Move(finalPos, finalRot, duration))
                .Join(endZoneView.GetZoneLayoutSequence(duration));
        }
        
        public Sequence GetHealingSequence(List<CardView> targets, float duration = 0.5f)
        {
            var healSequence = DOTween.Sequence();
            var animationTime = 0f;
            
            // TODO: Hack to move these out of the loop, maybe just pass in Source instead?
            var startZoneView = _gameView.FindZoneView(targets[0].Card.Owner, Zones.Discard);
            var endZoneView = _gameView.FindZoneView(targets[0].Card.Owner, Zones.Deck);

            targets = targets
                .OrderBy(c => c.Card.Data.Type)
                .ThenBy(c => c.Card.GetLessonType())
                .ToList();
            
            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                
                var targetPos = ZoneView.GetPosition(HealingPreviewPosition, i, HealingPreviewSpacing, HealingPreviewColumnCount);
                var targetRot = ZoneView.GetRotation(isFaceDown: false, isHorizontal: false, isEnemy: false);
                
                healSequence.Insert(animationTime, target.Move(targetPos, targetRot, duration));
                
                _gameView.ChangeZoneView(target, to: Zones.Deck, from: Zones.Discard);
                animationTime += 0.25f;
            }
            
            healSequence
                //.AppendInterval(duration * targets.Count * 0.5f)
                .Append(startZoneView.GetZoneLayoutSequence(duration))
                .Join(endZoneView.GetZoneLayoutSequence(duration));
            
            return healSequence;
        }
        private IEnumerator ShuffleDeckAnimation(IContainer container, GameAction action)
        {
            yield return true;
            
            var shuffleAction = (ShuffleDeckAction) action;
            var sequence = DOTween.Sequence().AppendInterval(0.5f).AppendInterval(0f);
            
            foreach (var target in shuffleAction.Targets)
            {
                var cardViews = _gameView.FindCardViews(target.Deck);
                
                for (var i = 0; i < cardViews.Count; i++)
                {
                    var cardView = cardViews[i];
                    
                    var offsetX = i % 2 == 1 ? 6f : -6f;
                    var offsetZ = -2f; 
                    
                    var offsetPos = cardView.transform.position + offsetX * Vector3.right + offsetZ * Vector3.forward;

                    var leftOffsetRot = new Vector3(5.5f, 16f, -6);
                    var rightOffsetRot = new Vector3(5.5f, -16f, 6);
                    
                    var targetRot = offsetX < 0f ? leftOffsetRot : rightOffsetRot;

                    if (target == _gameView.Match.EnemyPlayer)
                    {
                        targetRot.z += 180f;
                    }
                    
                    sequence.Join(cardView.Move(offsetPos, targetRot, 0.6f));
                }
            }
            
            while (sequence.IsPlaying())
            {
                yield return null;
            }

            sequence = DOTween.Sequence();
            
            foreach (var target in shuffleAction.Targets)
            {
                var zoneView = _gameView.FindZoneView(target, Zones.Deck);
                var targetRot = zoneView.GetRotation();
                var startDelay = 0f;
                for (var i = 0; i < target.Deck.Count; i++)
                {
                    var card = target.Deck[i];
                    var cardView = _gameView.FindCardView(card);
                    
                    
                    var finalPos = zoneView.GetPosition(i);

                    sequence.Join(cardView.Move(finalPos, targetRot, 0.3f, startDelay));
                    startDelay += i%2 == 1 ? 0.05f : 0f;
                }
            }

            while (sequence.IsPlaying())
            {
                yield return null;
            } 
        }

        private IEnumerator DamageCreatureAnimation(IContainer container, GameAction action)
        {
            var damageAction = (DamageCreatureAction) action;
            
            var particleSequence = _gameView.GetParticleSequence(damageAction, damageAction.Target);
                
            while (particleSequence.IsPlaying())
            {
                yield return null;
            }
        }
        
        private IEnumerator DamagePlayerAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var damageAction = (DamagePlayerAction) action;
            
            var target = damageAction.Target[Zones.Characters].First(); // NOTE: Should always be the starting character.

            var particleSequence = _gameView.GetParticleSequence(damageAction, target);
            
            while (particleSequence.IsPlaying())
            {
                yield return null;
            }
            
            var discardedCards = _gameView.FindCardViews(damageAction.DiscardedCards);
            
            for (var i = discardedCards.Count - 1; i >= 0; i--)
            {
                var cardView = discardedCards[i];
                var sequence = _gameView.GetMoveToZoneSequence(cardView, Zones.Discard, Zones.Deck);
                while (sequence.IsPlaying())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator DiscardAnimation(IContainer container, GameAction action)
        {
            var discardAction = (DiscardAction) action;
            
            if (!(discardAction.SourceAction is DamageCreatureAction))
            {
                var particleSequence = _gameView.GetParticleSequence(discardAction, discardAction.DiscardedCards);
                
                while (particleSequence.IsPlaying())
                {
                    yield return null;
                }
            }

            if (discardAction.DiscardedCards.Count > 0)
            {
                var cardViews = _gameView.FindCardViews(discardAction.DiscardedCards);

                // TODO: Cards could come from multiple zones, but we need to capture the before zones for each card for the animation.
                var fromZone = discardAction.DiscardedCards.Select(c => c.Zone).Distinct().Single(); 
            
                yield return true;
            
                foreach (var cardView in cardViews)
                {
                    var sequence = _gameView.GetMoveToZoneSequence(cardView, Zones.Discard, fromZone);
                    while (sequence.IsPlaying())
                    {
                        yield return null;
                    }
                }
            }
        }

        private IEnumerator PlayToBoardAnimation(IContainer container, GameAction action)
        {
            var playAction = (PlayToBoardAction) action;
    
            var particleSequence = _gameView.GetParticleSequence(playAction, playAction.Cards);

            while (particleSequence.IsPlaying())
            {
                yield return null;
            }
            
            var cardViewPairs = _gameView.FindCardViews(playAction.Cards)
                .Select(view => (view, view.Card.Data.Type.ToTargetZone()))
                .ToList();
            
            var fromZone = cardViewPairs.Select(v => v.view.Card.Zone).Distinct().Single();
            
            yield return true;
            
            var sequence = _gameView.GetMoveToZoneSequence(cardViewPairs, fromZone); 
            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }
        
        private IEnumerator HealingAnimation(IContainer container, GameAction action)
        {
            var healAction = (HealingAction) action;
            yield return true;

            var cardViews = _gameView.FindCardViews(healAction.HealedCards);

            var sequence = GetHealingSequence(cardViews);
            
            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }
        
        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            
            Global.Events.Unsubscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamagePlayer);
            Global.Events.Unsubscribe(Notification.Prepare<DamageCreatureAction>(), OnPrepareDamageCreature);
            Global.Events.Unsubscribe(Notification.Prepare<HealingAction>(), OnPrepareHealing);
            
            Global.Events.Unsubscribe(Notification.Prepare<ShuffleDeckAction>(), OnPrepareShuffleDeck);
        }
    }
}
