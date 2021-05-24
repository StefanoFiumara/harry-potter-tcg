using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class PlayEffectTargetingState : BaseTargetingState
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