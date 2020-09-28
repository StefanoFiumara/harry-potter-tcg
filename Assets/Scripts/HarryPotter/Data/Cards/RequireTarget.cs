using System;
using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards
{
    //NOTE: Serializable so that we can see it in the Inspector
    [Serializable]
    public class Mark
    {
        public Alliance Alliance;
        public Zones Zones;
    }
    
    //TODO: Support Multiple targets
    public class RequireTarget : CardAttribute
    {
        public Mark Preferred;
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
    }
}