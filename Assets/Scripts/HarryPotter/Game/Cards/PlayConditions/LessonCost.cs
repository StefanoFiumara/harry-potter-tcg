using HarryPotter.Enums;
using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Game.Cards.PlayConditions
{
    public class LessonCost : PlayCondition
    {
        [Range(0, 20)]
        public int Amount;
        public LessonType Type;
        
        public override bool MeetsCondition(PlayerState owner, PlayerState enemy)
        {
            return owner.LessonTypes.Contains(Type)
                   && owner.LessonCount >= Amount;
        }

        public override void Satisfy(PlayerState owner, PlayerState enemy)
        {
            // Do nothing
        }
    }
}