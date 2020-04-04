using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class LessonCost : RestrictionAttribute
    {
        [Range(0, 20)]
        public int Amount;
        public LessonType Type;

        [HideInInspector] public int DefaultAmount;

        public override void InitAttribute()
        {
            DefaultAmount = Amount;
        }

        public override void ResetAttribute()
        {
            Amount = DefaultAmount;
        }

        public override bool MeetsRestriction(Player owner)
        {
            var hasEnoughLessons = owner.LessonCount >= Amount;
            var hasLessonType = owner.LessonTypes.Contains(Type);

            return hasEnoughLessons && hasLessonType;
        }
    }
}