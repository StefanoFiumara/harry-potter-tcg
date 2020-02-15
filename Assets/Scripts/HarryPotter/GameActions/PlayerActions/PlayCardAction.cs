using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.PlayerActions
{
    public class PlayCardAction : GameAction
    {
        public bool UsePlayerAction { get; }
        public Card Card { get; }

        public PlayCardAction(Card card, bool usePlayerAction = false)
        {
            Card = card;
            Player = card.Owner;
            UsePlayerAction = usePlayerAction;
        }
    }
}