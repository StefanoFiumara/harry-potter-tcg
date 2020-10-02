using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class PlayCardAction : GameAction
    {
        public Card Card { get; }

        public PlayCardAction(Card card)
        {
            Card = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"PlayCardAction - Player {Player.Index} plays {Card.Data.CardName}";
        }
    }
}