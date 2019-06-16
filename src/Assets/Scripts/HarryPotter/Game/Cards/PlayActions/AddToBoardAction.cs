using HarryPotter.Enums;
using HarryPotter.Game.Player;

namespace HarryPotter.Game.Cards.PlayActions
{
    public class AddToBoardAction : PlayAction
    {
        public Zone TargetZone;
        
        public override void Execute(PlayerState owner, PlayerState enemy)
        {
            //TODO: Change this card's zone to TargetZone.
            //TODO: hmmm... we kinda need the card state here.
        }
    }
}