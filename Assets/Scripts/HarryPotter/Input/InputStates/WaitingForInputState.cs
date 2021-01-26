using System.Text;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class WaitingForInputState : BaseInputState, IClickableHandler, ITooltipContent
    {
        public void OnClickNotification(object sender, object args)
        {
            var gameStateMachine = InputSystem.Game.GetSystem<StateMachine>();
            var cardSystem = InputSystem.Game.GetSystem<CardSystem>();

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
            
            var playerOwnsCard = cardView.Card.Owner.Index == InputSystem.Game.Match.CurrentPlayerIndex;
            var cardInHand = cardView.Card.Zone == Zones.Hand;
            
                
            var clickData = (PointerEventData) args;
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                if (playerOwnsCard && cardInHand || cardView.Card.Zone.IsInBoard())
                {
                    gameStateMachine.ChangeState<PlayerInputState>();
                    
                    InputSystem.ActiveCard = cardView;
                    InputSystem.StateMachine.ChangeState<PreviewState>();                    
                }
            }
            
            else if (clickData.button == PointerEventData.InputButton.Left)
            {
                // TODO: Handle cases like activating a card's effect here (?)
                if (playerOwnsCard && cardInHand)
                {
                    InputSystem.ActiveCard = cardView;

                    InputSystem.PlayConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayCondition);
                    InputSystem.PlayEffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayEffect);

                    InputSystem.PlayConditionsIndex = 0;
                    InputSystem.PlayEffectsIndex = 0;

                    if (InputSystem.PlayConditionSelectors.Count > 0 && cardSystem.IsPlayable(cardView.Card))
                    {
                        InputSystem.StateMachine.ChangeState<PlayConditionTargetingState>();
                    }
                    else if (InputSystem.PlayEffectSelectors.Count > 0 && cardSystem.IsPlayable(cardView.Card))
                    {
                        InputSystem.StateMachine.ChangeState<PlayEffectTargetingState>();
                    }   
                    else
                    {
                        var action = new PlayCardAction(cardView.Card);
                        InputSystem.Game.Perform(action);
                        
                        InputSystem.StateMachine.ChangeState<ResetState>();
                    }
                }
            }
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            var cardSystem = InputSystem.Game.GetSystem<CardSystem>();
            var match = InputSystem.GameView.Match;
            
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