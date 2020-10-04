using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class TargetingState : BaseControllerState, IClickableHandler
    {
        private List<CardView> Targets { get; set; }

        private ManualTarget TargetAttribute { get; set; }
        
        public override void Enter()
        {
            Debug.Log("Entered Targeting State");
            TargetAttribute = Controller.ActiveCard.Card.GetAttribute<ManualTarget>();
            
            Controller.ActiveCard.Highlight(Color.yellow);
            
            Targets = new List<CardView>();
            TargetAttribute.Selected = new List<Card>();
        }
        
        public void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == Controller.ActiveCard)
            {
                if (Targets.Count >= TargetAttribute.RequiredAmount)
                {
                    PlayActiveCard();
                }
                else
                {
                    CancelTargeting();
                }
            }
            else if (cardView != null)
            {
                HandleTarget(cardView);
            }
        }

        private void HandleTarget(CardView cardView)
        {
            var targetSystem = Controller.Game.GetSystem<TargetSystem>();
            var candidates = targetSystem.GetTargetCandidates(cardView.Card, TargetAttribute.Allowed);

            if (!candidates.Contains(cardView.Card))
            {
                return;
            }

            if (Targets.Contains(cardView))
            {
                RemoveTarget(cardView);
            }
            else if (Targets.Count < TargetAttribute.MaxAmount)
            {
                AddTarget(cardView);
            }
        }

        private void AddTarget(CardView cardView)
        {
            Debug.Log($"Selected target {cardView.Card.Data.CardName}");

            cardView.Highlight(Color.red);
            Targets.Add(cardView);

            if (Targets.Count >= TargetAttribute.RequiredAmount)
            {
                Controller.ActiveCard.Highlight(Color.green);
            }
        }

        private void RemoveTarget(CardView cardView)
        {
            Debug.Log($"Deselected target {cardView.Card.Data.CardName}");
            cardView.Highlight(Color.clear);
            Targets.Remove(cardView);

            if (Targets.Count < TargetAttribute.RequiredAmount)
            {
                Controller.ActiveCard.Highlight(Color.yellow);
            }
        }

        private void CancelTargeting()
        {
            Targets.Clear();
            Controller.ActiveCard.Highlight(Color.clear);
            Controller.StateMachine.ChangeState<ResetState>();
        }

        private void PlayActiveCard()
        {
            Debug.Log($"Playing {Controller.ActiveCard.Card.Data.CardName} with targets: {string.Join(", ", Targets.Select(t => t.Card.Data.CardName))}.");
            
            Controller.ActiveCard.Highlight(Color.clear);

            foreach (var card in Targets)
            {
                card.Highlight(Color.clear);
            }

            TargetAttribute.Selected = Targets.Select(t => t.Card).ToList();
            Targets.Clear();

            var action = new PlayCardAction(Controller.ActiveCard.Card);
            Controller.Game.Perform(action);
            Controller.StateMachine.ChangeState<ResetState>();
        }

        public override void Exit()
        {
            Targets = null;
            TargetAttribute = null;
        }
    }
}