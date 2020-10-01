using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.PlayerActions
{
    public class SpellAction : GameAction
    {
        public Card Card { get; }

        public SpellAction(Card card)
        {
            Card = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"SpellAction - Player {Player.Index} casts spell {Card.Data.CardName}";
        }
    }
}