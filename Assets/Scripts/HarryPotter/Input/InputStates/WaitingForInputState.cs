using HarryPotter.Enums;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class WaitingForInputState : BaseControllerState, IClickableHandler
    {
        public void OnClickNotification(object sender, object args)
        {
            var gameStateMachine = Owner.Game.GetSystem<StateMachine>();

            if (!(gameStateMachine.CurrentState is PlayerIdleState))
            {
                return;
            }

            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            //TODO: Does this state need to interact with any other clickables that aren't cards?
            if (cardView == null
                || cardView.Card.Zone != Zones.Hand // TODO: And what about cards that are not in the player's hands?
                || cardView.Card.Owner.Index != Owner.Game.GameState.CurrentPlayerIndex) // TODO: And what about the opponent's cards?
            {
                return;
            }

            var clickData = (PointerEventData) args;
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                gameStateMachine.ChangeState<PlayerInputState>();
                Owner.ActiveCard = cardView;
                Owner.StateMachine.ChangeState<PreviewState>();                
            }
            
            //TODO: If regular click -> play card
            
        }
    }
}