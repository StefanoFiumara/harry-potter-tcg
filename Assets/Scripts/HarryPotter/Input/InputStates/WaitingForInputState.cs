using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Systems;
using HarryPotter.UI;
using HarryPotter.UI.Tooltips;
using HarryPotter.Utils;
using HarryPotter.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class WaitingForInputState : BaseControllerState, IClickableHandler, ITooltipContent
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
                    
                    if (cardView.Card.GetAttribute<ManualTarget>() != null && cardSystem.IsPlayable(cardView.Card))
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

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            var cardSystem = Controller.Game.GetSystem<CardSystem>();
            var match = Controller.GameView.Match;
            
            var tooltipText = new StringBuilder();
            
            if (context != null && context is CardView cardView)
            {
                if (cardSystem.IsPlayable(cardView.Card) && match.CurrentPlayerIndex == match.LocalPlayer.Index)
                {
                    tooltipText.Append($"{TextIcons.MOUSE_LEFT} Play - ");
                }
                    
                tooltipText.AppendLine($"{TextIcons.MOUSE_RIGHT} View");
            }

            return tooltipText.ToString();
        }
    }
}