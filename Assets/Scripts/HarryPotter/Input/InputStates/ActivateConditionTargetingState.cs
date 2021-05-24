using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class ActivateConditionTargetingState : CancelableTargetingState
    {
        public override void Enter()
        {
            TargetSelector = InputController.ConditionSelectors[InputController.ConditionsIndex];
            base.Enter();
        }
        
        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputController.ConditionsIndex > InputController.ConditionSelectors.Count - 1)
            {
                InputController.ConditionsIndex++;
                InputController.StateMachine.ChangeState<ActivateConditionTargetingState>();
            }
            else if (InputController.EffectSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<ActivateEffectTargetingState>();
            }
            else
            {
                var action = new ActivateCardAction(InputController.ActiveCard.Card);
                Debug.Log("*** PLAYER ACTIVATES CARD EFFECT ***");
                InputController.Game.Perform(action);
            
                InputController.StateMachine.ChangeState<ResetState>();
            }
        }
    }
}