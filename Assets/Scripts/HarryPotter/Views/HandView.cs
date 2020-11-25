using System.Collections;
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
    public class HandView : MonoBehaviour
    {
        private static readonly Vector3 SpellPreviewPos = new Vector3(0f, 0f, 40f);
        private static readonly Vector3 SpellPreviewRot = new Vector3(0f, 180f, 0f);
        
        private GameViewSystem _gameView;
        
        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<ReturnToHandAction>(), OnPrepareReturnToHand);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            
            _gameView = GetComponent<GameViewSystem>();

            if (_gameView == null)
            {
                Debug.LogError("BoardView could not find GameView");
            }
        }

        private void OnPrepareDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;
            action.PerformPhase.Viewer = DrawCardAnimation;
        }
        
        private void OnPrepareReturnToHand(object sender, object args)
        {
            var action = (ReturnToHandAction) args;
            action.PerformPhase.Viewer  = ReturnToHandAnimation;
        }
        
        private void OnPreparePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.Card.Data.Type == CardType.Spell)
            {
                action.PerformPhase.Viewer = SpellPreviewAnimation;
            }
        }
        
        private IEnumerator SpellPreviewAnimation(IContainer container, GameAction action)
        {
            var playCardAction = (PlayCardAction) action;
            var cardView = _gameView.FindCardView(playCardAction.Card);
            var handZone = _gameView.FindZoneView(playCardAction.Player, Zones.Hand);
            var discardZone = _gameView.FindZoneView(playCardAction.Player, Zones.Discard);
            
            var targetPos = discardZone.GetNextPosition();
            var targetRot = discardZone.GetRotation();
            
            _gameView.ChangeZoneView(cardView, Zones.Discard, from: Zones.Hand);
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
        
        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;

            var cardViews = _gameView.FindCardViews(drawAction.DrawnCards);
            
            //IMPORTANT: Animating through this list backwards animates the cards in the right order when there's more than one card to take from the pile.
            for (var i = cardViews.Count - 1; i >= 0; i--)
            {
                var cardView = cardViews[i];
                var anim = _gameView.MoveToZoneAnimation(cardView, Zones.Hand, Zones.Deck);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }
        
        private IEnumerator ReturnToHandAnimation(IContainer game, GameAction action)
        {
            var returnAction = (ReturnToHandAction) action;

            if (returnAction.Source.Data.Type == CardType.Spell)
            {
                var sequence = DOTween.Sequence();

                foreach (var returnedCard in returnAction.ReturnedCards)
                {
                    if (returnAction.Source == returnedCard)
                    {
                        // TODO: Difference effect for this case?
                        continue;
                    }

                    var particleType = returnAction.Source.GetAttribute<LessonCost>().Type;
                    var particleSequence = _gameView.GetParticleSequence(returnAction.Player, returnedCard, particleType);
                    sequence.Append(particleSequence);
                }
                
                while (sequence.IsPlaying())
                {
                    yield return null;
                }
            }
            
            var cardViews = _gameView.FindCardViews(returnAction.ReturnedCards);

            foreach (var cardView in cardViews)
            {
                var anim = _gameView.MoveToZoneAnimation(cardView, Zones.Hand);
                while (anim.MoveNext())
                {
                    yield return null;
                }
            }
        }
        
        private void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<ReturnToHandAction>(), OnPrepareReturnToHand);
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
        }
    }
}