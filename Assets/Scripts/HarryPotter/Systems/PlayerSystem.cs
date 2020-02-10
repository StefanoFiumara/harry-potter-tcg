using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Systems
{
    public class PlayerSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Subscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
        }

        private void OnPerformDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;

            if (action.UsePlayerAction)
            {
                action.Player.ActionsAvailable--;
            }
            
            action.Cards = action.Player[Zones.Deck]
                .Draw(action.Amount)
                .ToPlayerZone(action.Player, Zones.Hand);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.GameState.Players[action.NextPlayerIndex];
            DrawCards(player, 1);
            //TODO: Creature Damage Phase goes here
        }

        public void DrawCards(Player player, int amount, bool usePlayerAction = false)
        {
            var action = new DrawCardsAction(player, amount, usePlayerAction);
            if(Container.GetSystem<ActionSystem>().IsActive)
                Container.AddReaction(action);
            else 
                Container.Perform(action);
        }


        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Unsubscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
        }
    }
}