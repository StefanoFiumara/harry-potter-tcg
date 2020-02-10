using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class ActionCost : CardAttribute
    {
        [Range(0, 2)]
        public int Amount;

        [HideInInspector] public int DefaultAmount;

        private void Awake()
        {
            Amount = DefaultAmount;
        }

        public override void ResetAttribute()
        {
            Amount = DefaultAmount;
        }
    }
}