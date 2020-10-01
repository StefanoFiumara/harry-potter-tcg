using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;

namespace HarryPotter.Input.InputStates
{
    public class ResetState : BaseControllerState
    {
        public override void Enter()
        {
            Controller.StateMachine.ChangeState<WaitingForInputState>();
            Controller.Game.ChangeState<PlayerIdleState>();
        }
    }
}