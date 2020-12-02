using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.UI;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class TargetingState : BaseInputState, IClickableHandler, ITooltipContent
    {
        private List<CardView> _targets;
        private List<CardView> _targetCandidates;

        private ManualTarget _targetAttribute;

        private ZoneView _zoneInPreview;

        public override void Enter()
        {
            _targetAttribute = InputSystem.ActiveCard.Card.GetAttribute<ManualTarget>();
            
            InputSystem.ActiveCard.Highlight(_targetAttribute.RequiredAmount == 0 ? Colors.HasTargets : Colors.NeedsTargets);
            
            _targets = new List<CardView>();
            _targetAttribute.Selected = new List<Card>();
         
            var targetSystem = InputSystem.Game.GetSystem<TargetSystem>();
            var candidates = targetSystem.GetTargetCandidates(InputSystem.ActiveCard.Card, _targetAttribute.Allowed);

            _targetCandidates = InputSystem.GameView.FindCardViews(candidates);

            _targetCandidates.Highlight(Colors.IsTargetCandidate);

            if (_targetAttribute.Allowed.Zones.HasZone(Zones.Deck | Zones.Discard | Zones.Hand))
            {
                // NOTE: We only expect one of the above zones to be targetable at once, bad assumption?
                var player = _targetCandidates.Select(c => c.Card.Owner).Distinct().Single();
                var zoneToPreview = _targetCandidates.Select(c => c.Card.Zone).Distinct().Single();

                if (player.Index != MatchData.LOCAL_PLAYER_INDEX || zoneToPreview != Zones.Hand)
                {
                    var zoneView = InputSystem.GameView.FindZoneView(player, zoneToPreview);
                    zoneView.GetPreviewSequence();
                    _zoneInPreview = zoneView;
                }
            }
        }

        public void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();
            
            var clickData = (PointerEventData) args;
            
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                if (cardView == InputSystem.ActiveCard)
                {
                    CancelTargeting();
                }
                return;
            }
            
            if (cardView == InputSystem.ActiveCard)
            {
                if (_targets.Count >= _targetAttribute.RequiredAmount)
                {
                    PlayActiveCard();
                }
                else
                {
                    CancelTargeting();
                }
            }
            else if (cardView != null)
            {
                HandleTarget(cardView);
            }
        }

        private void HandleTarget(CardView cardView)
        {
            if (!_targetCandidates.Contains(cardView))
            {
                return;
            }

            if (_targets.Contains(cardView))
            {
                RemoveTarget(cardView);
            }
            else if (_targets.Count < _targetAttribute.MaxAmount)
            {
                AddTarget(cardView);
            }
        }

        private void AddTarget(CardView cardView)
        {
            cardView.Highlight(Colors.IsTargeted);
            _targets.Add(cardView);

            if (_targets.Count >= _targetAttribute.RequiredAmount)
            {
                InputSystem.ActiveCard.Highlight(Colors.HasTargets);
            }
        }

        private void RemoveTarget(CardView cardView)
        {
            var highlightColor = _targetCandidates.Contains(cardView) 
                ? Colors.IsTargetCandidate
                : Color.clear;
            
            cardView.Highlight(highlightColor);

            _targets.Remove(cardView);

            if (_targets.Count < _targetAttribute.RequiredAmount)
            {
                InputSystem.ActiveCard.Highlight(Colors.NeedsTargets);
            }
        }

        private void CancelTargeting()
        {
            _targets.Clear();
            
            InputSystem.ActiveCard.Highlight(Color.clear);
            _targetCandidates.Highlight(Color.clear);

            if (_zoneInPreview != null)
            {
                _zoneInPreview.GetZoneLayoutSequence();
                _zoneInPreview = null;
            }

            InputSystem.StateMachine.ChangeState<ResetState>();
        }

        private void PlayActiveCard()
        {
            InputSystem.ActiveCard.Highlight(Color.clear);

            _targetCandidates.Highlight(Color.clear);

            _targetAttribute.Selected = _targets.Select(t => t.Card).ToList();
            _targets.Clear();
            
            // TODO: Could this mess with animations for the targeted cards?
            if (_zoneInPreview != null)
            {
                _zoneInPreview.GetZoneLayoutSequence();
                _zoneInPreview = null;
            }

            var action = new PlayCardAction(InputSystem.ActiveCard.Card);
            InputSystem.Game.Perform(action);
            InputSystem.StateMachine.ChangeState<ResetState>();
        }

        public override void Exit()
        {
            _targets = null;
            _targetAttribute = null;
            _targetCandidates = null;
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            if (context != null && context is CardView cardView)
            {
                if (_targetCandidates.Contains(cardView))
                {
                    return _targets.Contains(cardView) 
                        ? $"{TextIcons.MOUSE_LEFT} Cancel Target" 
                        : $"{TextIcons.MOUSE_LEFT} Target";
                }

                if (InputSystem.ActiveCard == cardView)
                {
                    return _targets.Count >= _targetAttribute.RequiredAmount 
                        ? $"{TextIcons.MOUSE_LEFT} Play - {TextIcons.MOUSE_RIGHT} Cancel" 
                        : $"{TextIcons.MOUSE_LEFT}/{TextIcons.MOUSE_RIGHT} Cancel";
                }
            }

            return string.Empty;
        }
    }
}