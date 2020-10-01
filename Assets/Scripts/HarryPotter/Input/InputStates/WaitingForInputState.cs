using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Systems;
using HarryPotter.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace HarryPotter.Input.InputStates
{
    public class WaitingForInputState : BaseControllerState, IClickableHandler
    {
        public void OnClickNotification(object sender, object args)
        {
            var gameStateMachine = Controller.Game.GetSystem<StateMachine>();
            var cardSystem = Controller.Game.GetSystem<CardSystem>();

            if (!(gameStateMachine.CurrentState is PlayerIdleState))
            {
                return;
            }

            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == null)
            {
                return;
            }
            
            var playerOwnsCard = cardView.Card.Owner.Index == Controller.Game.Match.CurrentPlayerIndex;
            var cardInHand = cardView.Card.Zone == Zones.Hand;
            
                
            var clickData = (PointerEventData) args;
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                if (playerOwnsCard && cardInHand || cardView.Card.Zone.IsInBoard())
                {
                    gameStateMachine.ChangeState<PlayerInputState>();
                    
                    Controller.ActiveCard = cardView;
                    Controller.StateMachine.ChangeState<PreviewState>();                    
                }
            }
            
            else if (clickData.button == PointerEventData.InputButton.Left)
            {
                // TODO: Handle cases like activating a card's effect here (?)
                if (playerOwnsCard && cardInHand)
                {
                    Controller.ActiveCard = cardView;
                    
                    if (cardView.Card.GetAttribute<RequireTarget>() != null && cardSystem.IsPlayable(cardView.Card))
                    {
                        Controller.StateMachine.ChangeState<TargetingState>();
                    }
                    else
                    {
                        var action = new PlayCardAction(cardView.Card);
                        Controller.Game.Perform(action);
                        
                        Controller.StateMachine.ChangeState<ResetState>();
                    }
                }
            }
        }
    }
}