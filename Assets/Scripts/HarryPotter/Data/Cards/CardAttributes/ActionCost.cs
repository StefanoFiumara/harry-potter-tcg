using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class ActionCost : RestrictionAttribute
    {
        [Range(0, 2)]
        public int Amount;

        private int DefaultAmount { get; set; }

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
            return owner.ActionsAvailable >= Amount;
        }
    }
}