using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.StateManagement.GameStates
{
    public class GameOverState : BaseState
    {
        public override void Enter()
        {
            Debug.Log("*** Game Over! ***");
            Global.Events.Publish(VictorySystem.GAME_OVER_NOTIFICATION);
        }

        public override bool CanTransition(IState other)
        {
            //TODO: Can we transition anywhere from here?
            return false;
        }
    }
}