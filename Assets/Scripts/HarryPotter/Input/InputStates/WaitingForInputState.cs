using System.Text;
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
            var gameStateMachine = InputController.Game.GetSystem<StateMachine>();
            var cardSystem = InputController.Game.GetSystem<CardSystem>();
            var clickData = (PointerEventData) args;

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
            
            var playerOwnsCard = cardView.Card.Owner.Index == InputController.Game.GetMatch().CurrentPlayerIndex;
            
            InputController.SetActiveCard(cardView);
            InputController.ConditionsIndex = 0;
            InputController.EffectsIndex = 0;
            
            if (clickData.button == PointerEventData.InputButton.Right)
            {
                PreviewCard(cardView);
            }
            else if (clickData.button == PointerEventData.InputButton.Left)
            {
                if (!playerOwnsCard) return;
                
                if (cardSystem.IsPlayable(cardView.Card))
                {
                    PlayCard(cardView);
                }
                else if (cardSystem.IsActivatable(cardView.Card))
                {
                    ActivateCard(cardView);
                }
            }
        }

        private void PreviewCard(CardView cardView)
        {
            var playerOwnsCard = cardView.Card.Owner.Index == InputController.Game.GetMatch().CurrentPlayerIndex;
            var cardInHand = cardView.Card.Zone == Zones.Hand;
            var gameStateMachine = InputController.Game.GetSystem<StateMachine>();
            
            if (playerOwnsCard && cardInHand || cardView.Card.Zone.IsInPlay())
            {
                gameStateMachine.ChangeState<PlayerInputState>();

                InputController.SetActiveCard(cardView);
                InputController.StateMachine.ChangeState<PreviewState>();
            }
        }

        private void PlayCard(CardView cardView)
        {
            InputController.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayCondition);
            InputController.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayEffect);
            
            
            if (InputController.ConditionSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<PlayConditionTargetingState>();
            }
            else if (InputController.EffectSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<PlayEffectTargetingState>();
            }
            else
            {
                var action = new PlayCardAction(cardView.Card);
                Debug.Log("*** PLAYER ACTION ***");
                InputController.Game.Perform(action);

                InputController.StateMachine.ChangeState<ResetState>();
            }
        }

        private void ActivateCard(CardView cardView)
        {
            InputController.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateCondition);
            InputController.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateEffect);

            if (InputController.ConditionSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<ActivateConditionTargetingState>();
            }
            else if (InputController.EffectSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<ActivateEffectTargetingState>();
            }
            else
            {
                var action = new ActivateCardAction(cardView.Card);
                Debug.Log("*** PLAYER ACTIVATES CARD EFFECT ***");
                InputController.Game.Perform(action);

                InputController.StateMachine.ChangeState<ResetState>();
            }
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            var cardSystem = InputController.Game.GetSystem<CardSystem>();
            var match = InputController.GameView.Match;

            var isPlayerTurn = match.CurrentPlayerIndex == match.LocalPlayer.Index;
            
            var tooltipText = new StringBuilder();
            
            if (context != null && context is CardView cardView)
            {
                if (isPlayerTurn)
                {
                    var verb =
                        cardSystem.IsPlayable(cardView.Card) ? "Play" :
                        cardSystem.IsActivatable(cardView.Card) ? "Activate" 
                        : string.Empty;

                    if (!string.IsNullOrEmpty(verb))
                    {
                        tooltipText.Append($"{TextIcons.MOUSE_LEFT} {verb} - ");
                    }
                }
                    
                tooltipText.AppendLine($"{TextIcons.MOUSE_RIGHT} View");
            }

            return tooltipText.ToString();
        }
    }
}