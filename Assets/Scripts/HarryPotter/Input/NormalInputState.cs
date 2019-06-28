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

        public override IState HandleInput(RaycastHit selection)
        {
            var card = selection.transform.GetComponent<CardView>();
            if (card != null)
            {
                return HandleCardSelected(card);
            }

            // TODO: Check other game elements (deck, discard pile view (?))
            return this;
        }

        private IState HandleCardSelected(CardView card)
        {
            if (GameView.IsCardPlayable(card))
            {
                var fromHandReq = card.GetCardAttribute<FromHandTargetRequirement>();
                if (fromHandReq != null)
                {
                    return new TargetingInputState(InputHandler, GameView, card, TargetingType.Hand);
                }

                GameView.PlayCard(card);
            }
            else if (GameView.IsCardActivatable(card))
            {
                var fromPlayReq = card.GetCardAttribute<FromPlayTargetRequirement>();
                if (fromPlayReq != null)
                {
                    return new TargetingInputState(InputHandler, GameView, card, TargetingType.Effect);
                }

                GameView.ActivateCard(card);
            }

            return this;
        }
    }
}