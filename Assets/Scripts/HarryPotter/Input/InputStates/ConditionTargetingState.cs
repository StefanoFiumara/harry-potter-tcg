namespace HarryPotter.Input.InputStates
{
    public class ConditionTargetingState : CancelableTargetingState
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
                InputController.StateMachine.ChangeState<ConditionTargetingState>();
            }
            else if (InputController.EffectSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<EffectTargetingState>();
            }
            else
            {
                InputController.PerformDesiredAction();
            }
        }

    }
}
