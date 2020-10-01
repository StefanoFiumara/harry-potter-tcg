using HarryPotter.Data.Cards;
using HarryPotter.Enums;

namespace HarryPotter.GameActions.PlayerActions
{
    public class PlayToBoardAction : GameAction
    {
        public Card Card { get; }

        public PlayToBoardAction(Card card)
        {
            Card = card;
            Player = card.Owner;
        }

        public override string ToString()
        {
            return $"PlayToBoardAction - Player {Player.Index} plays {Card.Data.CardName} to Zone {Card.Data.Type.ToTargetZone()}";
        }
    }
}