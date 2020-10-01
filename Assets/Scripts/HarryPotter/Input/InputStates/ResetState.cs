using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Systems;

namespace HarryPotter.Input.InputStates
{
    public class ResetState : BaseControllerState
    {
        public override void Enter()
        {
            Controller.StateMachine.ChangeState<WaitingForInputState>();
            
            if (!Controller.Game.GetSystem<ActionSystem>().IsActive)
            {
                Controller.Game.ChangeState<PlayerIdleState>();
            }
        }
    }
}