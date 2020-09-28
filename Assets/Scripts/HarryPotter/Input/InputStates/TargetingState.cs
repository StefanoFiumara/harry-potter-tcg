using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems;
using HarryPotter.Views;
using UnityEngine;
using Utils;

namespace HarryPotter.Input.InputStates
{
    public class TargetingState : BaseControllerState, IClickableHandler
    {
        private List<CardView> Targets { get; set; }

        public override void Enter()
        {
            Debug.Log("Entered Targeting State");
            var target = Owner.ActiveCard.Card.GetAttribute<RequireTarget>();
            
            Owner.ActiveCard.Highlight(Color.yellow);
            
            Targets = new List<CardView>();
            target.Selected = new List<Card>();
        }
        
        public void OnClickNotification(object sender, object args)
        {
            var targetAttr = Owner.ActiveCard.Card.GetAttribute<RequireTarget>();
            var targetSystem = Owner.Game.GetSystem<TargetSystem>();

            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == Owner.ActiveCard)
            {
                if (Targets.Count >= targetAttr.RequiredAmount)
                {
                    Debug.Log($"Playing {cardView.Card.Data.CardName} with targets: {string.Join(", ", Targets.Select(t => t.Card.Data.CardName))}.");
                
                    Owner.ActiveCard.Highlight(Color.clear);

                    foreach (var card in Targets)
                    {
                        card.Highlight(Color.clear);
                    }

                    targetAttr.Selected = Targets.Select(t => t.Card).ToList();
                    Targets.Clear();
                
                    var action = new PlayCardAction(cardView.Card);
                    Owner.StateMachine.ChangeState<ResetState>();
                    Owner.Game.Perform(action);
                }
                else
                {
                    // Cancel
                    Targets.Clear();
                    Owner.ActiveCard.Highlight(Color.clear);
                    Owner.StateMachine.ChangeState<ResetState>();
                }
            }
            else if (cardView != null)
            {
                var candidates = targetSystem.GetMarks(targetAttr, targetAttr.Allowed);
                if (!candidates.Contains(cardView.Card)) return;
                
                if (Targets.Contains(cardView))
                {
                    Debug.Log($"Deselected target {cardView.Card.Data.CardName}");
                    cardView.Highlight(Color.clear);
                    Targets.Remove(cardView);
                    
                    if (Targets.Count < targetAttr.RequiredAmount)
                    {
                        Owner.ActiveCard.Highlight(Color.yellow);
                    }
                }
                else
                {
                    Debug.Log($"Selected target {cardView.Card.Data.CardName}");

                    if (Targets.Count >= targetAttr.MaxAmount)
                    {
                        return;
                    }
                    
                    cardView.Highlight(Color.red);
                    Targets.Add(cardView);

                    if (Targets.Count >= targetAttr.RequiredAmount)
                    {
                        Owner.ActiveCard.Highlight(Color.green);
                    }
                }
            }
        }
    }
}