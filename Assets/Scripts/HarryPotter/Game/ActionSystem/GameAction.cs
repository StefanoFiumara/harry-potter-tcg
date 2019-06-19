using System.Collections.Generic;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;

namespace HarryPotter.Game
{
    public class GameAction
    {
        public CardView Card;
        public List<CardView> Targets;
        

        public GameAction(ActionType action, CardView card, List<CardView> targets = null)
        {
            Card = card;
            Targets = targets ?? new List<CardView>();

            var actionMap = new Dictionary<ActionType, List<CardAction>>
            {
                {ActionType.FromHand, card.Data.PlayActions },
                {ActionType.InPlayEffect, card.Data.ActivateActions },
                {ActionType.InPlayReaction, card.Data.Reactions },
            };

            CardActions = actionMap[action];
        }

        public IEnumerable<CardAction> CardActions;
    }
}