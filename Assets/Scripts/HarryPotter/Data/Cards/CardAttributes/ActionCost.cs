using UnityEngine;
using UnityEngine.Serialization;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class ActionCost : CardAttribute
    {
        [Range(0, 2)]
        public int PlayCost;

        [Range(0, 2)]
        public int ActivateCost;

        private int DefaultAmount { get; set; }

        public override void InitAttribute()
        {
            DefaultAmount = PlayCost;
        }

        public override void ResetAttribute()
        {
            PlayCost = DefaultAmount;
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<ActionCost>();
            copy.PlayCost = PlayCost;
            copy.InitAttribute();
            return copy;
        }
    }
}