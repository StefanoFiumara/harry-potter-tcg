using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class ActivateCardAction : GameAction
    {
        public Card Card { get; }

        public ActivateCardAction(Card card)
        {
            Card = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"ActivateCardAction - {Player.PlayerName} activates {Card.Data.CardName}'s effect.";
        }
    }
}