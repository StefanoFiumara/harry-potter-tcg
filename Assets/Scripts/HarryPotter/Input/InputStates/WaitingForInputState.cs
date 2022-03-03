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
            var gameStateMachine = InputController.Game.GetSystem<StateMachine>();
            var cardSystem = InputController.Game.GetSystem<CardSystem>();
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
            InputController.ConditionsIndex = 0;
            InputController.EffectsIndex = 0;
            InputController.RewardsIndex = 0;

            if (clickData.button == PointerEventData.InputButton.Right)
            {
                PreviewCard(cardView);
            }
            else if (clickData.button == PointerEventData.InputButton.Left)
            {
                AssignInputParameters(cardSystem, cardView);

                if (InputController.DesiredAction != null)
                {
                    PerformInputAction();
                }
            }
        }

        private void AssignInputParameters(CardSystem cardSystem, CardView cardView)
        {
            if (cardSystem.IsPlayable(cardView.Card))
            {
                InputController.SetDesiredAction(new PlayCardAction(cardView.Card));

                InputController.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayCondition);
                InputController.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.PlayEffect);
                InputController.RewardSelectors = new List<ManualTargetSelector>();
            }
            // TODO: Can adventures be both activatable AND solvable?
            else if (cardSystem.IsActivatable(cardView.Card))
            {
                InputController.SetDesiredAction(new ActivateCardAction(cardView.Card));

                InputController.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateCondition);
                InputController.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.ActivateEffect);
                InputController.RewardSelectors = new List<ManualTargetSelector>();

            }
            else if (cardSystem.IsSolvable(cardView.Card))
            {
                InputController.SetDesiredAction(new SolveAdventureAction(cardView.Card, InputController.Game.GetMatch().CurrentPlayer));

                InputController.ConditionSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.AdventureSolveCondition);
                InputController.EffectSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.AdventureSolveEffect);
                InputController.RewardSelectors = cardView.Card.GetTargetSelectors<ManualTargetSelector>(AbilityType.AdventureReward);
            }
        }

        private void PerformInputAction()
        {
            if (InputController.ConditionSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<ConditionTargetingState>();
            }
            else if (InputController.EffectSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<EffectTargetingState>();
            }
            else if (InputController.RewardSelectors.Count > 0)
            {
                InputController.StateMachine.ChangeState<RewardsTargetingState>();
            }
            else
            {
                InputController.PerformDesiredAction();
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
