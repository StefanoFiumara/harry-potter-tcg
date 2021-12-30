using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class EffectTargetingState : BaseTargetingState
    {
        public override void Enter()
        {
            TargetSelector = InputController.EffectSelectors[InputController.EffectsIndex];
            base.Enter();
        }

        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputController.EffectsIndex > InputController.EffectSelectors.Count - 1)
            {
                InputController.EffectsIndex++;
                InputController.StateMachine.ChangeState<EffectTargetingState>();

            }
            else if (InputController.RewardSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<RewardsTargetingState>();
            }
            else
            {
                InputController.PerformDesiredAction();
            }
        }
    }
}
