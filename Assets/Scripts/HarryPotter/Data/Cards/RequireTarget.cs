using System;
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
        public bool IsRequired;
        public Mark Preferred;
        public Mark Allowed;
        
        public Card Selected { get; set; }
        
        public override void InitAttribute()
        {
            Selected = null;
        }

        public override void ResetAttribute()
        {
            Selected = null;
        }
    }
}