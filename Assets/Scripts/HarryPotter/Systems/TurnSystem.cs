using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class TurnSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<BeginGameAction>(), OnBeginGame);
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }

        private void OnBeginGame(object sender, object args)
        {
            //TODO: Randomly determine starting player (coin toss?) 
            var firstTurn = new ChangeTurnAction(Container.Match.CurrentPlayerIndex);
            Container.AddReaction(firstTurn);
        }

        public void ChangeTurn()
        {
            var action = new ChangeTurnAction(1 - Container.Match.CurrentPlayerIndex);
            Container.Perform(action);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            Container.Match.CurrentPlayerIndex = action.NextPlayerIndex;
            Container.Match.CurrentPlayer.ActionsAvailable = 2;
        }
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<BeginGameAction>(), OnBeginGame);
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