using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class LessonCost : CardAttribute
    {
        [Range(0, 20)]
        public int Amount;
        public LessonType Type;

        public int DefaultAmount { get; private set; }

        public override void InitAttribute()
        {
            DefaultAmount = Amount;
        }

        public override void ResetAttribute()
        {
            Amount = DefaultAmount;
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<LessonCost>();
            copy.Amount = Amount;
            copy.Type = Type;
            copy.InitAttribute();
            return copy;
        }
    }
}