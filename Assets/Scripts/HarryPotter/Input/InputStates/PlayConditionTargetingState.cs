using System.Linq;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class PlayEffectTargetingState : BaseTargetingState, ITooltipContent
    {
        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            var action = new PlayCardAction(InputSystem.ActiveCard.Card);
            InputSystem.Game.Perform(action);
            
            InputSystem.StateMachine.ChangeState<ResetState>();
        }
        
        public string GetActionText(MonoBehaviour context = null)
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
        
        public string GetDescriptionText() => string.Empty;
    }
    
    public class PlayConditionTargetingState : BaseTargetingState, ITooltipContent
    {
        public override void OnClickNotification(object sender, object args)
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
                if (Targets.Count < TargetSelector.RequiredAmount)
                {
                    CancelTargeting();
                }
            }

            base.OnClickNotification(sender, args);
        }

        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();
            
            InputSystem.StateMachine.ChangeState<PlayEffectTargetingState>();
        }

        private void CancelTargeting()
        {
            Targets.Clear();
            
            InputSystem.ActiveCard.Highlight(Color.clear);
            CandidateViews.Highlight(Color.clear);

            if (ZoneInPreview != null)
            {
                ZoneInPreview.GetZoneLayoutSequence();
                ZoneInPreview = null;
            }

            InputSystem.StateMachine.ChangeState<ResetState>();
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
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
                        ? $"{TextIcons.MOUSE_LEFT} Play - {TextIcons.MOUSE_RIGHT} Cancel" 
                        : $"{TextIcons.MOUSE_LEFT}/{TextIcons.MOUSE_RIGHT} Cancel";
                }
            }
    
            return string.Empty;
        }
    }
}