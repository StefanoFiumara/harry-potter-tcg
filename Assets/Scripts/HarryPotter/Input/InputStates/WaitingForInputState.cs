using HarryPotter.Enums;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class WaitingForInputState : BaseControllerState, IClickableHandler
    {
        public void OnClickNotification(object sender, object args)
        {
            var currentGameState = Owner.Game.GetSystem<StateMachine>().CurrentState;

            if (!(currentGameState is PlayerIdleState)) return;

            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == null
                || cardView.Card.Zone != Zones.Hand 
                || cardView.Card.Owner.Index != Owner.Game.GameState.CurrentPlayerIndex)
                return;
            
            Debug.Log($"Switch to preview state for card {cardView.name}");
            Owner.Game.GetSystem<StateMachine>().ChangeState<PlayerInputState>();
            Owner.ActiveCard = cardView;
            Owner.StateMachine.ChangeState<ShowPreviewState>();
        }
    }
}