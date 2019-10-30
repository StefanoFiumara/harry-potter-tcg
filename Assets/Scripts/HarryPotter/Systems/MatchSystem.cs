using HarryPotter.Data;
using HarryPotter.GameActions;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class MatchSystem : Core.System
    {
        public GameState GameState;
        
        private void Start()
        {
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }

        public void ChangeTurn()
        {
            var action = new ChangeTurnAction(1 - GameState.CurrentPlayerIndex);
            Container.Perform(action);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            Debug.Log("Performing Change Turn");
            var action = (ChangeTurnAction) args;
            GameState.CurrentPlayerIndex = action.NextPlayerIndex;
        }
        
        private void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }
    }
}