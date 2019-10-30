using HarryPotter.Data;
using HarryPotter.GameActions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class MatchSystem : Core.System, IAwake, IDestroy
    {
        public GameState GameState;
        
        public void Awake()
        {
            if (GameState == null)
            {
                throw new UnityException("Match System did not receive GameState.");
            }
            
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
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }
    }
}