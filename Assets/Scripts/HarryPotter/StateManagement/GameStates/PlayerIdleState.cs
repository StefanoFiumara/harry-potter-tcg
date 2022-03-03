using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;

namespace HarryPotter.StateManagement.GameStates
{
    public class PlayerIdleState : BaseState
    {
        public override void Enter()
        {
            var aiSystem = Game.GetSystem<AISystem>();
            var cardSystem = Game.GetSystem<CardSystem>();

            cardSystem.Refresh(ControlMode.Local);

            if (aiSystem != null && Game.GetMatch().CurrentPlayerIndex == MatchData.ENEMY_PLAYER_INDEX)
            {
                aiSystem.UseAction();
            }
        }
    }
}
