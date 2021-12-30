using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class SolveAdventureAction : GameAction
    {
        public SolveAdventureAction(Card card)
        {
            SourceCard = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"SolveAdventureAction - {Player.PlayerName} solves the adventure {SourceCard.Data.CardName}.";
        }
    }
}