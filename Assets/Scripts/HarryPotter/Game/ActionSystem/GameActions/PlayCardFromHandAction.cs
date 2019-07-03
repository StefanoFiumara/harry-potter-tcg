using System.Collections.Generic;
using HarryPotter.Game.Cards;

namespace HarryPotter.Game.ActionSystem.GameActions
{
    public class PlayCardFromHandAction : ExecuteCardAction
    {
        public PlayCardFromHandAction(CardView card, List<CardView> targets = null)
            : base(card, targets, card.Data.PlayActions) { }
    }
}