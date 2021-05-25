using System.Collections;
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
    public class HandView : MonoBehaviour
    {
        private GameView _gameView;
        private BoardView _boardView;
        
        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<ReturnToHandAction>(), OnPrepareReturnToHand);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            
            _gameView = GetComponent<GameView>();

            _boardView = GetComponent<BoardView>();

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

            if (action.SourceCard.Data.Type == CardType.Spell)
            {
                action.PerformPhase.Viewer = SpellPreviewAnimation;
            }
        }
        
        private IEnumerator SpellPreviewAnimation(IContainer container, GameAction action)
        {
            var playCardAction = (PlayCardAction) action;
            var cardView = _gameView.FindCardView(playCardAction.SourceCard);
            
            //_gameView.ChangeZoneView(cardView, Zones.Discard, from: Zones.Hand);
            yield return true; //NOTE: Moves the card out of the Hand Zone

            var previewSequence = _boardView.GetRevealSequence(cardView, Zones.Discard, Zones.Hand);
            while (previewSequence.IsPlaying())
            {
                yield return null;
            }
        }


        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;

            foreach (var card in drawAction.DrawnCards)
            {
                var cardView = _gameView.FindCardView(card);

                // TODO: Only displaying previews for single card draws is a hack to prevent the preview animation from playing during the initial 7 card hand draw.
                //       Refactor to something that would allow the player to preview cards for multiple card draws after game begin.
                // TODO: Card Draw Preview flag in settings that could enable/disable this animation per user?
                if (cardView.Card.Owner.Index == _gameView.Match.LocalPlayer.Index && drawAction.DrawnCards.Count == 1)
                {
                    var previewSequence = _boardView.GetRevealSequence(cardView, Zones.Hand, Zones.Deck);
                    while (previewSequence.IsPlaying())
                    {
                        yield return null;
                    }
                }
                else
                {
                    var sequence = _gameView.GetMoveToZoneSequence(cardView, Zones.Hand, Zones.Deck);
                    while (sequence.IsPlaying())
                    {
                        yield return null;
                    }
                }
            }
        }

        private IEnumerator ReturnToHandAnimation(IContainer game, GameAction action)
        {
            var returnAction = (ReturnToHandAction) action;
            
            var particleSequence = _gameView.GetParticleSequence(returnAction, returnAction.ReturnedCards);
            while (particleSequence.IsPlaying())
            {
                yield return null;
            }

            //NOTE: Caching fromZones before yield true is necessary when cards are changing zones.
            var cardViews = _gameView.FindCardViews(returnAction.ReturnedCards);
            var fromZones = cardViews.Select(v => v.Card.Zone).ToList();
            
            yield return true;

            var sequence = DOTween.Sequence();
            
            for (var i = 0; i < cardViews.Count; i++)
            {
                var cardView = cardViews[i];
                var fromZone = fromZones[i];
                
                // TODO: GetRevealSequence to preview multiple cards (similar to healing preview animation)
                // TODO: Not every card that has this action requires a card to be revealed (e.g. Gringotts Vault Key), add "Reveal" flag to ReturnToHandAction?
                sequence.Join(fromZone.IsInPlay() 
                    ? _gameView.GetMoveToZoneSequence(cardView, Zones.Hand, fromZone)
                    : _boardView.GetRevealSequence(cardView, Zones.Hand, fromZone));
            }
            
            while (sequence.IsPlaying())
            {
                yield return null;
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