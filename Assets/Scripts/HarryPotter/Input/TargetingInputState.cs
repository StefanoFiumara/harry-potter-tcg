using HarryPotter.Game;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Cards.CardAttributes;
using UnityEngine;
using Utils;

namespace HarryPotter.Input
{
    internal class TargetingInputState : InputState
    {
        public TargetingInputState(InputHandler inputHandler, GameView gameView) : base(inputHandler, gameView) { }

        public override InputType HandleInput(RaycastHit selection)
        {
            var card = selection.transform.GetComponent<CardView>();
            if (card != null)
            {
                if (card == InputHandler.TargetSource)
                {
                    return HandleOriginalSelected(card);
                }

                if (CanBeTargeted(card))
                {
                    ToggleSelection(card);
                }
            }

            //TODO: Handle Targeting Enemy Player Deck
            return InputType.Targeting;
        }

        private void ToggleSelection(CardView card)
        {
            if (InputHandler.Targets.Contains(card))
            {
                InputHandler.Deselect(card);
            }
            else
            {
                InputHandler.Select(card);
            }
        }

        private bool CanBeTargeted(CardView card)
        {
            var requirement = InputHandler.TargetSource.GetTargetRequirement(InputHandler.TargetingType);

            if (InputHandler.Targets.Count >= requirement.NumberOfTargets) return false;

            var isMine = card.Owner == InputHandler.TargetSource.Owner;

            if (requirement.TargetableTypes.Contains(card.Data.Type))
            {
                if (requirement.CanTargetEnemy && !isMine || requirement.CanTargetOwn && isMine)
                {
                    return true;
                }
            }

            return false;
        }

        private InputType HandleOriginalSelected(CardView card)
        {
            if (InputHandler.TargetingType == TargetingType.Hand)
            {
                var req = card.GetCardAttribute<FromHandTargetRequirement>();

                if (req.HasEnoughTargets(InputHandler.Targets.Count))
                {
                    GameView.PlayCard(card, InputHandler.Targets);
                    InputHandler.Targets.Clear();
                    InputHandler.TargetSource = null;

                    return InputType.Normal;
                }
            }
            else if (InputHandler.TargetingType == TargetingType.Effect)
            {
                var req = card.GetCardAttribute<FromPlayTargetRequirement>();

                if (req.HasEnoughTargets(InputHandler.Targets.Count))
                {
                    GameView.ActivateCard(card, InputHandler.Targets);
                    InputHandler.Targets.Clear();
                    InputHandler.TargetSource = null;

                    return InputType.Normal;
                }
            }

            InputHandler.TargetSource = null;
            InputHandler.Targets.Clear();
            return InputType.Normal;
        }
    }
}