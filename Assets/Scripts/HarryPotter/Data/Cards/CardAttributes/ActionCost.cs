using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class ActionCost : CardAttribute
    {
        [Range(0, 2)]
        public int PlayCost;

        [Range(0, 2)]
        public int ActivateCost;

        private int DefaultPlayCost { get; set; }
        private int DefaultActivateCost { get; set; }

        public override void InitAttribute()
        {
            DefaultPlayCost = PlayCost;
            DefaultActivateCost = ActivateCost;
        }

        public override void ResetAttribute()
        {
            PlayCost = DefaultPlayCost;
            ActivateCost = DefaultActivateCost;
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<ActionCost>();
            copy.PlayCost = PlayCost;
            copy.ActivateCost = ActivateCost;
            copy.InitAttribute();
            return copy;
        }
    }
}
