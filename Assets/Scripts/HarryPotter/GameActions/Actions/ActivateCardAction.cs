using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class ActivateCardAction : GameAction
    {
        public ActivateCardAction(Card card)
        {
            SourceCard = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"ActivateCardAction - {Player.PlayerName} activates {SourceCard.Data.CardName}'s effect.";
        }
    }
}