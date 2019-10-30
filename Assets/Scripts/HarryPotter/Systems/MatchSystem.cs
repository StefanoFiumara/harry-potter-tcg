using HarryPotter.Data;
using HarryPotter.GameActions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class MatchSystem : GameSystem, IGameState, IAwake, IDestroy
    {
        public GameState GameState { get; set; }

        public void Awake()
        {
            GameState.Initialize();
            
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }

        public void ChangeTurn()
        {
            var action = new ChangeTurnAction(1 - GameState.CurrentPlayerIndex);
            Container.Perform(action);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            GameState.CurrentPlayerIndex = action.NextPlayerIndex;
            GameState.CurrentPlayer.ActionsAvailable += 2;
        }
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }
    }
}