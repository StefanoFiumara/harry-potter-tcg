using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Systems;

namespace HarryPotter.Input.InputStates
{
    public class ResetState : BaseInputState
    {
        public override void Enter()
        {
            InputSystem.StateMachine.ChangeState<WaitingForInputState>();
            
            if (!InputSystem.Game.GetSystem<ActionSystem>().IsActive)
            {
                InputSystem.Game.ChangeState<PlayerIdleState>();
            }
        }
    }
}