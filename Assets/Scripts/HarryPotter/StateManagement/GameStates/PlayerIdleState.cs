using HarryPotter.Data;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems;
using Utils;

namespace HarryPotter.StateManagement.GameStates
{
    public class PlayerIdleState : BaseState
    {
        public override void Enter()
        {
            var aiSystem = Container.GetSystem<AISystem>();
            
            if (aiSystem != null && Container.GameState.CurrentPlayerIndex == GameState.ENEMY_PLAYER_INDEX)
            {
                aiSystem.UseAction();
            }
        }
    }
}