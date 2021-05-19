using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class CastSpellAction : GameAction
    {
        public CastSpellAction(Card card)
        {
            SourceCard = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"CastSpellAction - {Player.PlayerName} casts spell {SourceCard.Data.CardName}.";
        }
    }
}