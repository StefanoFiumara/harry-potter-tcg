namespace HarryPotter.Input.InputStates
{
    public class RewardsTargetingState : BaseTargetingState
    {
        public override void Enter()
        {
            TargetSelector = InputController.RewardSelectors[InputController.RewardsIndex];
            base.Enter();
        }

        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputController.RewardsIndex > InputController.RewardSelectors.Count - 1)
            {
                InputController.RewardsIndex++;
                InputController.StateMachine.ChangeState<RewardsTargetingState>();

            }
            else
            {
                InputController.PerformDesiredAction();
            }
        }
    }
}