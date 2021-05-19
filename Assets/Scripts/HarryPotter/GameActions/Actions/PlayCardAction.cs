using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class PlayCardAction : GameAction
    {
        public PlayCardAction(Card card)
        {
            SourceCard = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"PlayCardAction - {Player.PlayerName} plays {SourceCard.Data.CardName}.";
        }
    }
}