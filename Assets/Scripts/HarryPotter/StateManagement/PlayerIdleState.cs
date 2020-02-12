using HarryPotter.Data;
using HarryPotter.Systems;

namespace HarryPotter.StateManagement
{
    public class PlayerIdleState : BaseState
    {
        public override void Enter()
        {
            var aiSystem = Container.GetSystem<AISystem>();
            
            if (aiSystem != null && Container.GameState.CurrentPlayerIndex == GameState.ENEMY_PLAYER_INDEX)
            {
                aiSystem.TakeTurn();
            }
        }
    }
}