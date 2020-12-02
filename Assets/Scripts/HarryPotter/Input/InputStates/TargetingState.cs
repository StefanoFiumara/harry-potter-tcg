using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
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
    public class TargetingState : BaseControllerState, IClickableHandler, ITooltipContent
    {
        public List<CardView> Targets { get; set; }
        public List<CardView> TargetCandidates { get; set; }
        
        private ManualTarget TargetAttribute { get; set; }

        public override void Enter()
        {
            TargetAttribute = Controller.ActiveCard.Card.GetAttribute<ManualTarget>();
            
            Controller.ActiveCard.Highlight(TargetAttribute.RequiredAmount == 0 ? Colors.HasTargets : Colors.NeedsTargets);
            
            Targets = new List<CardView>();
            TargetAttribute.Selected = new List<Card>();
         
            var targetSystem = Controller.Game.GetSystem<TargetSystem>();
            var candidates = targetSystem.GetTargetCandidates(Controller.ActiveCard.Card, TargetAttribute.Allowed);

            TargetCandidates = Controller.GameView.FindCardViews(candidates);

            TargetCandidates.Highlight(Colors.IsTargetCandidate);
        }
        
        public void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            
            var clickData = (PointerEventData) args;
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                return;
            }
            
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == Controller.ActiveCard)
            {
                if (Targets.Count >= TargetAttribute.RequiredAmount)
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
            if (!TargetCandidates.Contains(cardView))
            {
                return;
            }

            if (Targets.Contains(cardView))
            {
                RemoveTarget(cardView);
            }
            else if (Targets.Count < TargetAttribute.MaxAmount)
            {
                AddTarget(cardView);
            }
        }

        private void AddTarget(CardView cardView)
        {
            cardView.Highlight(Colors.IsTargeted);
            Targets.Add(cardView);

            if (Targets.Count >= TargetAttribute.RequiredAmount)
            {
                Controller.ActiveCard.Highlight(Colors.HasTargets);
            }
        }

        private void RemoveTarget(CardView cardView)
        {
            var highlightColor = TargetCandidates.Contains(cardView) 
                ? Colors.IsTargetCandidate
                : Color.clear;
            
            cardView.Highlight(highlightColor);

            Targets.Remove(cardView);

            if (Targets.Count < TargetAttribute.RequiredAmount)
            {
                Controller.ActiveCard.Highlight(Colors.NeedsTargets);
            }
        }

        private void CancelTargeting()
        {
            Targets.Clear();
            Controller.ActiveCard.Highlight(Color.clear);
            
            TargetCandidates.Highlight(Color.clear);
            
            Controller.StateMachine.ChangeState<ResetState>();
        }

        private void PlayActiveCard()
        {
            Controller.ActiveCard.Highlight(Color.clear);

            TargetCandidates.Highlight(Color.clear);

            TargetAttribute.Selected = Targets.Select(t => t.Card).ToList();
            Targets.Clear();

            var action = new PlayCardAction(Controller.ActiveCard.Card);
            Controller.Game.Perform(action);
            Controller.StateMachine.ChangeState<ResetState>();
        }

        public override void Exit()
        {
            Targets = null;
            TargetAttribute = null;
            TargetCandidates = null;
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            if (context != null && context is CardView cardView)
            {
                if (TargetCandidates.Contains(cardView))
                {
                    return Targets.Contains(cardView) 
                        ? $"{TextIcons.MOUSE_LEFT} Cancel Target" 
                        : $"{TextIcons.MOUSE_LEFT} Target";
                }
            }

            return string.Empty;
        }
    }
}