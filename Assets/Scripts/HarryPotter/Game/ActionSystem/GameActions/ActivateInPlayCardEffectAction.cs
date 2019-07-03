using System.Collections.Generic;
using HarryPotter.Game.Cards;

namespace HarryPotter.Game.ActionSystem.GameActions
{
    public class ActivateInPlayCardEffectAction : ExecuteCardAction
    {
        public ActivateInPlayCardEffectAction(CardView card, List<CardView> targets = null)
            : base(card, targets, card.Data.ActivateActions) { }
    }
}