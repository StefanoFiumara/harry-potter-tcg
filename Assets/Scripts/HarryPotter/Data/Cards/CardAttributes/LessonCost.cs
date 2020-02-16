using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class LessonCost : CardAttribute
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
    }
}