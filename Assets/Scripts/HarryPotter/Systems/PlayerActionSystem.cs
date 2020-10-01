using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Systems
{
    public class PlayerActionSystem : IGameSystem, IAwake, IDestroy
    {
        public IContainer Container { get; set; }


        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            
            Global.Events.Subscribe(Notification.Validate<DrawCardsAction>(), OnValidateDrawCards);
            Global.Events.Subscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
            
            Global.Events.Subscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.Match.Players[action.NextPlayerIndex];

            player.ActionsAvailable = 2;
        }

        private void OnValidateDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) sender;
            var validator = (Validator) args;

            if (action.UsePlayerAction && action.Player.ActionsAvailable < 1)
            {
                validator.Invalidate("Not enough actions");
            }
        }

        private void OnPerformDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;

            if (action.UsePlayerAction)
            {
                //TODO: we may not want to hard-code this value... are there cards that cause the player to use 2 actions to draw?
                action.Player.ActionsAvailable--;
            }
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;
            
            var actionCost = action.Card.GetAttribute<ActionCost>();
            
            if (action.Card.Owner.ActionsAvailable < actionCost.Amount)
            {
                validator.Invalidate("Not enough actions");
            }
        }

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            var actionCost = action.Card.GetAttribute<ActionCost>();
            action.Player.ActionsAvailable -= actionCost.Amount;
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
           
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
           
            Global.Events.Unsubscribe(Notification.Validate<DrawCardsAction>(), OnValidateDrawCards);
            Global.Events.Unsubscribe(Notification.Perform<DrawCardsAction>(), OnPerformDrawCards);
        }
    }
}