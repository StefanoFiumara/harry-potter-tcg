using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;

namespace HarryPotter.StateManagement.GameStates
{
    public class PlayerIdleState : BaseState
    {
        public override void Enter()
        {
            var aiSystem = Container.GetSystem<AISystem>();
            var cardSystem = Container.GetSystem<CardSystem>();
            
            cardSystem.Refresh(ControlMode.Local); 
            
            if (aiSystem != null && Container.GetMatch().CurrentPlayerIndex == MatchData.ENEMY_PLAYER_INDEX)
            {
                aiSystem.UseAction();
            }
        }
    }
}