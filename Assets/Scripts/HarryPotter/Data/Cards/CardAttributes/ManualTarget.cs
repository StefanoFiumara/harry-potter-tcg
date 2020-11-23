using System;
using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards.CardAttributes
{
    //NOTE: Serializable so that we can see it in the Inspector
    [Serializable]
    public class Mark
    {
        public Alliance Alliance;
        public Zones Zones;
        public CardType CardType;
        public LessonType LessonType;
    }
    
    public class ManualTarget : CardAttribute
    {
        public Mark Allowed;

        public int RequiredAmount;
        public int MaxAmount;
        
        public List<Card> Selected { get; set; }
        
        public override void InitAttribute()
        {
            Selected = new List<Card>();
        }

        public override void ResetAttribute()
        {
            Selected = new List<Card>();
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<ManualTarget>();
            copy.Allowed = Allowed;
            copy.RequiredAmount = RequiredAmount;
            copy.MaxAmount = MaxAmount;
            copy.InitAttribute();
            return copy;
        }
    }
}