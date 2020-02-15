using HarryPotter.Data;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems;
using Utils;

namespace HarryPotter.StateManagement
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
            else if (Container.GameState.CurrentPlayerIndex == GameState.LOCAL_PLAYER_INDEX)
            {
                if (Container.GameState.CurrentPlayer.ActionsAvailable > 0)
                {
                    var playAction = new PlayCardAction(Container.GameState.CurrentPlayer.Hand.TakeTop(), true);
                    Container.Perform(playAction);
                }
            }
        }
    }
}