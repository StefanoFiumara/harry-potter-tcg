using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class ActionCost : CardAttribute
    {
        [Range(0, 2)]
        public int Amount;
    }
}