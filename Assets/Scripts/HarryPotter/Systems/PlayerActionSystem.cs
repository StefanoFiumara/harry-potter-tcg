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
            var player = Container.GameState.Players[action.NextPlayerIndex];

            player.ActionsAvailable = 2; // TODO: send ValueChangedEvent
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
                //TODO: we may not want to hard-code this value.
                action.Player.ActionsAvailable--; // TODO: send ValueChangedEvent
            }
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;
            
            var actionCost = action.Card.GetAttribute<ActionCost>();
            
            if (!actionCost.MeetsRestriction(action.Player))
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