using System.Collections.Generic;
using DG.Tweening;
using HarryPotter.Enums;

namespace HarryPotter.Game.Cards.PlayActions
{
    public class MoveToZoneAction : CardAction
    {
        public Zone TargetZone;
        
        public override Sequence Execute(CardView card, List<CardView> targets)
        {
            var player = card.Owner;

            return player.MoveToZone(card, TargetZone);
        }
    }
}