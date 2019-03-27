using HarryPotter.Enums;
using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.Game.Data.PlayConditions
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
    }
}