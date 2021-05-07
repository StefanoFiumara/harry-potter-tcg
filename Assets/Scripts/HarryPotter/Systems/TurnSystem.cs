using HarryPotter.Data;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class TurnSystem : GameSystem, IAwake, IDestroy
    {
        private MatchData _match;
        
        public void Awake()
        {
            _match = Container.GetMatch();
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            _match.CurrentPlayerIndex = action.NextPlayerIndex;
            _match.CurrentPlayer.ActionsAvailable = 2;
        }

        public void ChangeTurn()
        {
            var action = new ChangeTurnAction(1 - _match.CurrentPlayerIndex);
            
            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);    
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }
    }

    public static class TurnSystemExtensions
    {
        public static void ChangeTurn(this IContainer game)
        {
            var matchSystem = game.GetSystem<TurnSystem>();
            matchSystem.ChangeTurn();
        }
    }
}