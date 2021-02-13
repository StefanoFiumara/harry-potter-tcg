using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class PlayEffectTargetingState : BaseTargetingState, ITooltipContent
    {
        public override void Enter()
        {
            TargetSelector = InputSystem.PlayEffectSelectors[InputSystem.PlayEffectsIndex];
            base.Enter();
        }

        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputSystem.PlayEffectsIndex > InputSystem.PlayEffectSelectors.Count - 1)
            {
                InputSystem.PlayEffectsIndex++;
                InputSystem.StateMachine.ChangeState<PlayEffectTargetingState>();
                
            }
            else
            {
                var action = new PlayCardAction(InputSystem.ActiveCard.Card);
                InputSystem.Game.Perform(action);
            
                InputSystem.StateMachine.ChangeState<ResetState>();
            }
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
}