using System.Collections;
using System.Linq;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Views
{
    public class HandView : MonoBehaviour
    {
        private GameView _gameView;

        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<ReturnToHandAction>(), OnPrepareReturnToHand);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            
            _gameView = GetComponent<GameView>();

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
            
            yield return true;

            var previewSequence = _gameView.GetRevealSequence(cardView, Zones.Discard, Zones.Hand);
            while (previewSequence.IsPlaying())
            {
                yield return null;
            }
        }

        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;
            
            var groupedViews = _gameView.FindCardViews(drawAction.DrawnCards).GroupBy(v => v.Card.Owner);

            foreach (var group in groupedViews)
            {
                var cardViews = group.ToList();
                var zoneViews = Enumerable.Repeat(Zones.Deck, cardViews.Count).ToList();
                
                // IDEA: Player might want to speed up their game by skipping the preview sequence for this animation
                //       Card Draw Preview flag in settings that could disable this behavior in favor of GetMoveToZoneSequence?
                if (group.Key == _gameView.Match.LocalPlayer)
                {
                    var sequence = _gameView.GetRevealSequence(cardViews, Zones.Hand, zoneViews);
                    while (sequence.IsPlaying())
                    {
                        yield return null;
                    }
                }
                else
                {
                    foreach (var view in cardViews)
                    {
                        var sequence = _gameView.GetMoveToZoneSequence(view, Zones.Hand, Zones.Deck);
                        while (sequence.IsPlaying())
                        {
                            yield return null;
                        }
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
            
            // TODO: Not every card that has this action requires a card to be revealed (e.g. Gringotts Vault Key)
            //       Add "Reveal" flag to ReturnToHandAction? Should "RevealAction" be part of the game systems?
            var views = _gameView.FindCardViews(returnAction.ReturnedCards);
            var revealSequence = _gameView.GetRevealSequence(views, Zones.Hand, fromZones);

            while (revealSequence.IsPlaying())
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