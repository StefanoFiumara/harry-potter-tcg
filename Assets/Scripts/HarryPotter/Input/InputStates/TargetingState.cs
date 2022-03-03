using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.Systems;
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
        protected List<CardView> Targets;
        protected List<CardView> CandidateViews;
        protected ManualTargetSelector TargetSelector;

        protected ZoneView ZoneInPreview;

        private TargetSystem _targetSystem;

        private bool IsTargetingPreviewZones => TargetSelector.Allowed.Zones.HasZone(Zones.Deck | Zones.Discard | Zones.Hand);

        public override void Enter()
        {
            _targetSystem = Game.GetSystem<TargetSystem>();

            Targets = new List<CardView>();

            TargetSelector = InputController.TargetSelectors[InputController.SelectorIndex];
            TargetSelector.Selected = new List<Card>();

            // TODO: System to display TargetSelector.FormattedTargetPrompt to the player

            var candidates = _targetSystem.GetTargetCandidates(InputController.ActiveCard.Card, TargetSelector.Allowed);
            CandidateViews = InputController.GameView.FindCardViews(candidates);

            InputController.ActiveCard.Highlight(TargetSelector.RequiredAmount == 0 ? Colors.HasTargets : Colors.NeedsTargets);
            CandidateViews.Highlight(Colors.IsTargetCandidate);

            if (IsTargetingPreviewZones)
            {
                // NOTE: We only expect one of the above zones to be targetable at once, bad assumption?
                var player = CandidateViews.Select(c => c.Card.Owner).Distinct().Single();
                var zoneToPreview = CandidateViews.Select(c => c.Card.Zone).Distinct().Single();

                if (player.Index != MatchData.LOCAL_PLAYER_INDEX || zoneToPreview != Zones.Hand)
                {
                    var zoneView = InputController.GameView.FindZoneView(player, zoneToPreview);
                    zoneView.GetPreviewSequence(sortOrder: PreviewSortOrder.ByType);
                    ZoneInPreview = zoneView;
                }
            }
        }

        public override void Exit()
        {
            Targets = null;
            CandidateViews = null;
            TargetSelector = null;
            _targetSystem = null;

            if (ZoneInPreview != null)
            {
                ZoneInPreview.GetZoneLayoutSequence();
                ZoneInPreview = null;
            }
        }

        public virtual void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            var clickData = (PointerEventData) args;

            if (clickData.button == PointerEventData.InputButton.Right)
            {
                return;
            }

            if (cardView == InputController.ActiveCard)
            {
                if (Targets.Count >= TargetSelector.RequiredAmount)
                {
                    HandleTargetsAcquired();
                }
            }
            else if (cardView != null)
            {
                HandleTarget(cardView);
            }
        }

        private void HandleTarget(CardView cardView)
        {
            if (!CandidateViews.Contains(cardView))
            {
                return;
            }

            if (Targets.Contains(cardView))
            {
                RemoveTarget(cardView);
            }
            else if (Targets.Count < TargetSelector.MaxAmount)
            {
                AddTarget(cardView);
            }
        }

        private void AddTarget(CardView cardView)
        {
            Targets.Add(cardView);
            cardView.Highlight(Colors.IsTargeted);
            cardView.SetTargetCounter(Targets.Count);

            if (Targets.Count >= TargetSelector.RequiredAmount)
            {
                InputController.ActiveCard.Highlight(Colors.HasTargets);
            }
        }

        private void RemoveTarget(CardView cardView)
        {
            var highlightColor = CandidateViews.Contains(cardView)
                ? Colors.IsTargetCandidate
                : Color.clear;

            cardView.Highlight(highlightColor);

            Targets.Remove(cardView);
            cardView.HideTargetCounter();
            UpdateTargetCounters();

            if (Targets.Count < TargetSelector.RequiredAmount)
            {
                InputController.ActiveCard.Highlight(Colors.NeedsTargets);
            }
        }

        private void UpdateTargetCounters()
        {
            for (var i = 0; i < Targets.Count; i++)
            {
                var target = Targets[i];
                target.SetTargetCounter(i + 1);
            }
        }

        private void ApplyTargetsToSelector()
        {
            InputController.ActiveCard.Highlight(Color.clear);

            CandidateViews.Highlight(Color.clear);
            Targets.ClearTargetCounters();


            TargetSelector.Selected = Targets.Select(t => t.Card).ToList();
            Targets.Clear();
        }

        private void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputController.SelectorIndex > InputController.TargetSelectors.Count - 1)
            {
                InputController.SelectorIndex++;

                if (InputController.SelectorIndex > InputController.ConditionCount - 1)
                {
                    Owner.ChangeState<TargetingState>();
                }
                else
                {
                    Owner.ChangeState<CancelableTargetingState>();
                }

            }
            else
            {
                InputController.PerformDesiredAction();
            }
        }

        public bool IsCandidateZone(Card card)
        {
            return TargetSelector.Allowed.Zones.HasZone(card.Zone);
        }

        public virtual string GetActionText(MonoBehaviour context = null)
        {
            if (context != null && context is CardView cardView)
            {
                if (CandidateViews.Contains(cardView))
                {
                    return Targets.Contains(cardView)
                        ? $"{TextIcons.MOUSE_LEFT} Cancel Target"
                        : $"{TextIcons.MOUSE_LEFT} Target";
                }

                if (InputController.ActiveCard == cardView)
                {
                    return Targets.Count >= TargetSelector.RequiredAmount
                        ? $"{TextIcons.MOUSE_LEFT} Play"
                        : string.Empty;
                }
            }

            return string.Empty;
        }

        public virtual string GetDescriptionText() => string.Empty;
    }
}
