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
    public abstract class BaseTargetingState : BaseInputState, IClickableHandler, ITooltipContent
    {
        protected List<CardView> Targets;
        protected List<CardView> CandidateViews;
        protected ManualTargetSelector TargetSelector;

        protected ZoneView ZoneInPreview;

        private TargetSystem _targetSystem;

        public override void Enter()
        {
            if (TargetSelector == null)
            {
                Debug.LogError($"Target selector not set for {GetType().Name}");
                return;
            }
            _targetSystem = InputSystem.Game.GetSystem<TargetSystem>();
            
            Targets = new List<CardView>();
            TargetSelector.Selected = new List<Card>();
         
            var candidates = _targetSystem.GetTargetCandidates(InputSystem.ActiveCard.Card, TargetSelector.Allowed);
            CandidateViews = InputSystem.GameView.FindCardViews(candidates);

            InputSystem.ActiveCard.Highlight(TargetSelector.RequiredAmount == 0 ? Colors.HasTargets : Colors.NeedsTargets);
            CandidateViews.Highlight(Colors.IsTargetCandidate);

            if (TargetSelector.Allowed.Zones.HasZone(Zones.Deck | Zones.Discard | Zones.Hand))
            {
                // NOTE: We only expect one of the above zones to be targetable at once, bad assumption?
                var player = CandidateViews.Select(c => c.Card.Owner).Distinct().Single();
                var zoneToPreview = CandidateViews.Select(c => c.Card.Zone).Distinct().Single();

                if (player.Index != MatchData.LOCAL_PLAYER_INDEX || zoneToPreview != Zones.Hand)
                {
                    var zoneView = InputSystem.GameView.FindZoneView(player, zoneToPreview);
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
            
            if (cardView == InputSystem.ActiveCard)
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
            cardView.Highlight(Colors.IsTargeted);
            Targets.Add(cardView);

            if (Targets.Count >= TargetSelector.RequiredAmount)
            {
                InputSystem.ActiveCard.Highlight(Colors.HasTargets);
            }
        }

        private void RemoveTarget(CardView cardView)
        {
            var highlightColor = CandidateViews.Contains(cardView) 
                ? Colors.IsTargetCandidate
                : Color.clear;
            
            cardView.Highlight(highlightColor);

            Targets.Remove(cardView);

            if (Targets.Count < TargetSelector.RequiredAmount)
            {
                InputSystem.ActiveCard.Highlight(Colors.NeedsTargets);
            }
        }

        protected void ApplyTargetsToSelector()
        {
            InputSystem.ActiveCard.Highlight(Color.clear);

            CandidateViews.Highlight(Color.clear);

            TargetSelector.Selected = Targets.Select(t => t.Card).ToList();
            Targets.Clear();
        }
        
        protected abstract void HandleTargetsAcquired();
        
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

                if (InputSystem.ActiveCard == cardView)
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