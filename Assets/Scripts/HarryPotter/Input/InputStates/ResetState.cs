using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Systems;

namespace HarryPotter.Input.InputStates
{
    public class ResetState : BaseInputState
    {
        public override void Enter()
        {
            InputController.ClearState();

            InputController.StateMachine.ChangeState<WaitingForInputState>();

            if (!Game.GetSystem<ActionSystem>().IsActive)
            {
                Game.ChangeState<PlayerIdleState>();
            }
        }
    }
}
