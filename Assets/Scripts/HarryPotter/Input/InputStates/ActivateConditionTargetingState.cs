using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class ActivateConditionTargetingState : CancelableTargetingState
    {
        public override void Enter()
        {
            TargetSelector = InputSystem.ConditionSelectors[InputSystem.ConditionsIndex];
            base.Enter();
        }
        
        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputSystem.ConditionsIndex > InputSystem.ConditionSelectors.Count - 1)
            {
                InputSystem.ConditionsIndex++;
                InputSystem.StateMachine.ChangeState<ActivateConditionTargetingState>();
            }
            else if (InputSystem.EffectSelectors.Count > 0)
            {
                InputSystem.StateMachine.ChangeState<PlayEffectTargetingState>();
            }
            else
            {
                var action = new ActivateCardAction(InputSystem.ActiveCard.Card);
                Debug.Log("*** PLAYER ACTIVATES CARD EFFECT ***");
                InputSystem.Game.Perform(action);
            
                InputSystem.StateMachine.ChangeState<ResetState>();
            }
        }
    }
}