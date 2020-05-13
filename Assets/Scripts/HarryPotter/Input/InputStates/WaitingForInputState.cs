using HarryPotter.Data.Cards;
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
            var gameStateMachine = Owner.Game.GetSystem<StateMachine>();
            var cardSystem = Owner.Game.GetSystem<CardSystem>();

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
            
            var playerOwnsCard = cardView.Card.Owner.Index == Owner.Game.GameState.CurrentPlayerIndex;
            var cardInHand = cardView.Card.Zone == Zones.Hand;
            
                
            var clickData = (PointerEventData) args;
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                if (playerOwnsCard && cardInHand || cardView.Card.Zone.IsInBoard())
                {
                    gameStateMachine.ChangeState<PlayerInputState>();
                    
                    Owner.ActiveCard = cardView;
                    Owner.StateMachine.ChangeState<PreviewState>();                    
                }
            }
            
            else if (clickData.button == PointerEventData.InputButton.Left)
            {
                // TODO: Handle cases like activating a card's effect here (?)
                if (playerOwnsCard && cardInHand)
                {
                    Owner.ActiveCard = cardView;
                    
                    if (cardView.Card.GetAttribute<RequireTarget>() != null && cardSystem.IsPlayable(cardView.Card))
                    {
                        Owner.StateMachine.ChangeState<TargetingState>();
                    }
                    else
                    {
                        var action = new PlayCardAction(cardView.Card);
                        Owner.StateMachine.ChangeState<ResetState>();
                        Owner.Game.Perform(action);                        
                    }
                }
            }
        }
    }
}