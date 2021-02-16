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
            var gameStateMachine = InputSystem.Game.GetSystem<StateMachine>();
            var cardSystem = InputSystem.Game.GetSystem<CardSystem>();
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
            
            var playerOwnsCard = cardView.Card.Owner.Index == InputSystem.Game.Match.CurrentPlayerIndex;
            
            InputSystem.ActiveCard = cardView;
            InputSystem.ConditionsIndex = 0;
            InputSystem.EffectsIndex = 0;
            
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
            var playerOwnsCard = cardView.Card.Owner.Index == InputSystem.Game.Match.CurrentPlayerIndex;
            var cardInHand = cardView.Card.Zone == Zones.Hand;
            var gameStateMachine = InputSystem.Game.GetSystem<StateMachine>();
            
            if (playerOwnsCard && cardInHand || cardView.Card.Zone.IsInPlay())
            {
                gameStateMachine.ChangeState<PlayerInputState>();

                InputSystem.ActiveCard = cardView;
                InputSystem.StateMachine.ChangeState<PreviewState>();
            }
        }

        private void PlayCard(CardView cardView)
        {
            InputSystem.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayCondition);
            InputSystem.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayEffect);
            
            
            if (InputSystem.ConditionSelectors.Count > 0)
            {
                InputSystem.StateMachine.ChangeState<PlayConditionTargetingState>();
            }
            else if (InputSystem.EffectSelectors.Count > 0)
            {
                InputSystem.StateMachine.ChangeState<PlayEffectTargetingState>();
            }
            else
            {
                var action = new PlayCardAction(cardView.Card);
                Debug.Log("*** PLAYER ACTION ***");
                InputSystem.Game.Perform(action);

                InputSystem.StateMachine.ChangeState<ResetState>();
            }
        }

        private void ActivateCard(CardView cardView)
        {
            InputSystem.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateCondition);
            InputSystem.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateEffect);

            var cardSystem = InputSystem.Game.GetSystem<CardSystem>();
            
            if (InputSystem.ConditionSelectors.Count > 0 && cardSystem.IsActivatable(cardView.Card))
            {
                InputSystem.StateMachine.ChangeState<ActivateConditionTargetingState>();
            }
            else if (InputSystem.EffectSelectors.Count > 0 && cardSystem.IsActivatable(cardView.Card))
            {
                InputSystem.StateMachine.ChangeState<ActivateEffectTargetingState>();
            }
            else
            {
                var action = new ActivateCardAction(cardView.Card);
                Debug.Log("*** PLAYER ACTIVATES CARD EFFECT ***");
                InputSystem.Game.Perform(action);

                InputSystem.StateMachine.ChangeState<ResetState>();
            }
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            var cardSystem = InputSystem.Game.GetSystem<CardSystem>();
            var match = InputSystem.GameView.Match;

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