using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;

namespace HarryPotter.Input.InputStates
{
    public class ResetState : BaseControllerState
    {
        public override void Enter()
        {
            Owner.StateMachine.ChangeState<WaitingForInputState>();
            Owner.Game.ChangeState<PlayerIdleState>();
        }
    }
}