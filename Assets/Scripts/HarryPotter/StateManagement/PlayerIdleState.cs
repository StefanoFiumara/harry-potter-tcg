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
            
            // *** Temporary - Testing PlayCardAction ***
            else if (Container.GameState.CurrentPlayerIndex == GameState.LOCAL_PLAYER_INDEX)
            {
                var currentPlayer = Container.GameState.CurrentPlayer;
                
                if (currentPlayer.ActionsAvailable > 0 && currentPlayer.Hand.Count > 0)
                {
                    var playAction = new PlayCardAction(currentPlayer.Hand.TakeRandom(), true);
                    Container.Perform(playAction);
                }
            }
        }
    }
}