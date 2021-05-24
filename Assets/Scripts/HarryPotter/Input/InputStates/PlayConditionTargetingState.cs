using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class PlayConditionTargetingState : CancelableTargetingState
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
                InputController.StateMachine.ChangeState<PlayConditionTargetingState>();
            }
            else if (InputController.EffectSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<PlayEffectTargetingState>();
            }
            else
            {
                var action = new PlayCardAction(InputController.ActiveCard.Card);
                Debug.Log("*** PLAYER ACTION ***");
                InputController.Game.Perform(action);
            
                InputController.StateMachine.ChangeState<ResetState>();
            }
        }
        
    }
}