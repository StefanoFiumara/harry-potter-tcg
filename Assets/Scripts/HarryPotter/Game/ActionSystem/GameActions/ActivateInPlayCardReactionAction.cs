using System.Collections.Generic;
using HarryPotter.Game.Cards;

namespace HarryPotter.Game.ActionSystem.GameActions
{
    public class ActivateInPlayCardReactionAction : ExecuteCardAction
    {
        public ActivateInPlayCardReactionAction(CardView card, List<CardView> targets = null)
            : base(card, targets, card.Data.Reactions) { }
    }
}