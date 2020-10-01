using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.PlayerActions
{
    public class CastSpellAction : GameAction
    {
        public Card Card { get; }

        public CastSpellAction(Card card)
        {
            Card = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"CastSpellAction - Player {Player.Index} casts spell {Card.Data.CardName}";
        }
    }
}