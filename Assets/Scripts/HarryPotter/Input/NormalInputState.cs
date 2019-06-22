using HarryPotter.Game;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Cards.CardAttributes;
using UnityEngine;
using Utils;

namespace HarryPotter.Input
{
    public class NormalInputState : InputState
    {
        public NormalInputState(InputHandler inputHandler, GameView gameView) : base(inputHandler, gameView) { }

        public override InputType HandleInput(RaycastHit selection)
        {
            var card = selection.transform.GetComponent<CardView>();
            if (card != null)
            {
                return HandleCardSelected(card);
            }

            // TODO: Check other game elements (deck, discard pile view (?))
            return InputType.Normal;
        }

        private InputType HandleCardSelected(CardView card)
        {
            if (GameView.IsCardPlayable(card))
            {
                var fromHandReq = card.GetCardAttribute<FromHandTargetRequirement>();
                if (fromHandReq != null && !fromHandReq.HasEnoughTargets(InputHandler.Targets.Count))
                {
                    InputHandler.TargetSource = card;
                    InputHandler.TargetingType = TargetingType.Hand;
                    return InputType.Targeting;
                }

                GameView.PlayCard(card, InputHandler.Targets);
                InputHandler.Targets.Clear();
            }
            else if (GameView.IsCardActivatable(card))
            {
                var fromPlayReq = card.GetCardAttribute<FromPlayTargetRequirement>();
                if (fromPlayReq != null && !fromPlayReq.HasEnoughTargets(InputHandler.Targets.Count))
                {
                    InputHandler.TargetSource = card;
                    InputHandler.TargetingType = TargetingType.Effect;
                    return InputType.Targeting;
                }

                GameView.ActivateCard(card, InputHandler.Targets);
                InputHandler.Targets.Clear();
            }

            return InputType.Normal;
        }
    }
}