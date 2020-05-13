using HarryPotter.Data.Cards;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems;
using HarryPotter.Views;
using UnityEngine;
using Utils;

namespace HarryPotter.Input.InputStates
{
    public class TargetingState : BaseControllerState, IClickableHandler
    {
        public override void Enter()
        {
            Debug.Log("Entered Targeting State");
            var target = Owner.ActiveCard.Card.GetAttribute<RequireTarget>();

            target.Selected = null;
        }

        public void OnClickNotification(object sender, object args)
        {
            var target = Owner.ActiveCard.Card.GetAttribute<RequireTarget>();
            
            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == Owner.ActiveCard)
            {
                Debug.Log($"Playing {cardView.Card.Data.CardName} with {target.Selected.Data.CardName} as target.");
                var action = new PlayCardAction(cardView.Card);
                Owner.StateMachine.ChangeState<ResetState>();
                Owner.Game.Perform(action);
                return;
            }
            
            if (cardView != null)
            {
                //TODO: How do we show that a card has been selected?
                target.Selected = cardView.Card;
            }
            else
            {
                target.Selected = null;
            }
        }
    }
}