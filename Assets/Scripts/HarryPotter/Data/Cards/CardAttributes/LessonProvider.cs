using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class LessonProvider : CardAttribute 
    {
        public int Amount;
        public LessonType Type;

        private int DefaultAmount { get; set; }

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
            var copy = CreateInstance<LessonProvider>();
            copy.Amount = Amount;
            copy.Type = Type;
            copy.InitAttribute();
            return copy;
        }
    }
}