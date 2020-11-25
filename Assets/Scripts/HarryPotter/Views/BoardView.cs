using System.Collections;
using System.Linq;
using DG.Tweening;
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
    public class BoardView : MonoBehaviour
    {
        private GameViewSystem _gameView;

        private void Awake()
        {
            
            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamagePlayer);
            Global.Events.Subscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
            
            
            _gameView = GetComponent<GameViewSystem>();

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

        private void OnPrepareDamagePlayer(object sender, object args)
        {
            var action = (DamagePlayerAction) args;
            action.PerformPhase.Viewer = DamagePlayerAnimation;
        }

        private void OnPrepareDiscard(object sender, object args)
        {
            var action = (DiscardAction) args;
            action.PerformPhase.Viewer  = DiscardAnimation;
        }
        
        private IEnumerator DamagePlayerAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var damageAction = (DamagePlayerAction) action;

            // TODO: Consolidate these cases
            if (damageAction.Source.Data.Type == CardType.Spell)
            {
                var target = damageAction.Target[Zones.Characters].First(); // NOTE: Should always be the starting character.
                var particleType = damageAction.Source.GetAttribute<LessonCost>().Type;
                var particleSequence = _gameView.GetParticleSequence(damageAction.Player, target, particleType);

                while (particleSequence.IsPlaying())
                {
                    yield return null;
                }                    
            }
            else if (damageAction.Source.Data.Type == CardType.Creature)
            {
                var target = damageAction.Target[Zones.Characters].First(); // NOTE: Should always be the starting character.
                var particleSequence = _gameView.GetParticleSequence(damageAction.Source, target);

                while (particleSequence.IsPlaying())
                {
                    yield return null;
                }    
            }
            
            var discardedCards = _gameView.FindCardViews(damageAction.DiscardedCards);
            
            for (var i = discardedCards.Count - 1; i >= 0; i--)
            {
                var cardView = discardedCards[i];
                var anim = _gameView.MoveToZoneAnimation(cardView, Zones.Discard, Zones.Deck);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator DiscardAnimation(IContainer container, GameAction action)
        {
            var discardAction = (DiscardAction) action;

            if (discardAction.Source.Data.Type == CardType.Spell)
            {
                var sequence = DOTween.Sequence();

                foreach (var discardedCard in discardAction.DiscardedCards)
                {
                    // IMPORTANT: This check needs to be done so that spell cards that discard themselves don't do the particle animation 
                    if (discardAction.Source == discardedCard)
                    {
                        continue;
                    }

                    var particleType = discardAction.Source.GetAttribute<LessonCost>().Type;
                    var particleSequence = _gameView.GetParticleSequence(discardAction.Player, discardedCard, particleType);
                    sequence.Append(particleSequence);
                }
                
                while (sequence.IsPlaying())
                {
                    yield return null;
                }
            }
            
            var cardViews = _gameView.FindCardViews(discardAction.DiscardedCards);

            foreach (var cardView in cardViews)
            {
                var anim = _gameView.MoveToZoneAnimation(cardView, Zones.Discard);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator PlayToBoardAnimation(IContainer container, GameAction action)
        {
            var playAction = (PlayToBoardAction) action;

            var cardViewPairs = _gameView.FindCardViews(playAction.Cards)
                .Select(view => (view, view.Card.Data.Type.ToTargetZone()))
                .ToList();
            
            
            var anim = _gameView.MoveToZoneAnimation(cardViewPairs);
            while (anim.MoveNext())
            {
                yield return anim.Current;
            }
        }
        
        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Prepare<DamagePlayerAction>(), OnPrepareDamagePlayer);
            Global.Events.Unsubscribe(Notification.Prepare<DiscardAction>(), OnPrepareDiscard);
        }
    }
}