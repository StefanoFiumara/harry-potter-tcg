using System.Collections.Generic;
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
            var gameStateMachine = Game.GetSystem<StateMachine>();
            var cardSystem = Game.GetSystem<CardSystem>();
            var clickData = (PointerEventData) args;

            if (gameStateMachine.CurrentState is not PlayerIdleState)
            {
                return;
            }

            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == null)
            {
                return;
            }

            InputController.SetActiveCard(cardView);

            InputController.TargetSelectors = new List<ManualTargetSelector>();
            InputController.SelectorIndex = 0;

            if (clickData.button == PointerEventData.InputButton.Right)
            {
                PreviewCard(cardView);
            }
            else if (clickData.button == PointerEventData.InputButton.Left)
            {
                DetermineDesiredAction(cardSystem, cardView);

                if (InputController.DesiredAction != null)
                {
                    PerformInputAction();
                }
            }
        }

        private void DetermineDesiredAction(CardSystem cardSystem, CardView cardView)
        {
            if (cardSystem.IsPlayable(cardView.Card))
            {
                var conditions = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayCondition);
                var effects = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayEffect);

                InputController.TargetSelectors.AddRange(conditions);
                InputController.TargetSelectors.AddRange(effects);

                InputController.ConditionCount = conditions.Count;
                InputController.SetDesiredAction(new PlayCardAction(cardView.Card));
            }
            // TODO: Can adventures be both activatable AND solvable?
            else if (cardSystem.IsActivatable(cardView.Card))
            {
                var conditions = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateCondition);
                var effects = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateEffect);

                InputController.TargetSelectors.AddRange(conditions);
                InputController.TargetSelectors.AddRange(effects);

                InputController.ConditionCount = conditions.Count;
                InputController.SetDesiredAction(new ActivateCardAction(cardView.Card));

            }
            else if (cardSystem.IsSolvable(cardView.Card))
            {
                var conditions = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.AdventureSolveCondition);
                var effects = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.AdventureSolveEffect);
                var rewards = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.AdventureReward);

                InputController.TargetSelectors.AddRange(conditions);
                InputController.TargetSelectors.AddRange(effects);
                InputController.TargetSelectors.AddRange(rewards);

                InputController.ConditionCount = conditions.Count;
                InputController.SetDesiredAction(new SolveAdventureAction(cardView.Card, Game.GetMatch().CurrentPlayer));
            }
        }

        private void PerformInputAction()
        {
            if (InputController.TargetSelectors.Count > 0)
            {
                Owner.ChangeState<TargetingState>();
            }
            else
            {
                InputController.PerformDesiredAction();
            }
        }

        private void PreviewCard(CardView cardView)
        {
            var playerOwnsCard = cardView.Card.Owner.Index == Game.GetMatch().CurrentPlayerIndex;
            var cardInHand = cardView.Card.Zone == Zones.Hand;
            var gameStateMachine = Game.GetSystem<StateMachine>();

            if (playerOwnsCard && cardInHand || cardView.Card.Zone.IsInPlay())
            {
                gameStateMachine.ChangeState<PlayerInputState>();

                InputController.SetActiveCard(cardView);
                Owner.ChangeState<PreviewState>();
            }
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            var cardSystem = Game.GetSystem<CardSystem>();
            var match = Game.GetMatch();

            var isPlayerTurn = match.CurrentPlayerIndex == match.LocalPlayer.Index;

            var tooltipText = new StringBuilder();

            if (context != null && context is CardView cardView)
            {
                if (isPlayerTurn)
                {
                    var verb =
                        cardSystem.IsPlayable(cardView.Card) ? "Play" :
                        cardSystem.IsActivatable(cardView.Card) ? "Activate" :
                        cardSystem.IsSolvable(cardView.Card) ? "Solve" :
                        string.Empty;

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
