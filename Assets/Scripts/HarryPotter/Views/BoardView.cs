using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    public class BoardView : MonoBehaviour
    {
        private GameView _gameView;
        
        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            
            Global.Events.Subscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamagePlayer);
            Global.Events.Subscribe(Notification.Prepare<DamageCreatureAction>(), OnPrepareDamageCreature);
            
            
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

        private void OnPrepareShuffleDeck(object sender, object args)
        {
            var action = (ShuffleDeckAction) args;
            action.PerformPhase.Viewer  = ShuffleDeckAnimation;
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
            
            foreach (var cardView in discardedCards)
            {
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
        
        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            
            Global.Events.Unsubscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamagePlayer);
            Global.Events.Unsubscribe(Notification.Prepare<DamageCreatureAction>(), OnPrepareDamageCreature);

            Global.Events.Unsubscribe(Notification.Prepare<ShuffleDeckAction>(), OnPrepareShuffleDeck);
        }
    }
}
