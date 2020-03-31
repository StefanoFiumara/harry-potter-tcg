using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.PlayerActions
{
    public class PlayCardAction : GameAction
    {
        public Card Card { get; }

        public PlayCardAction(Card card)
        {
            Card = card;
            Player = card.Owner;
        }
    }
}