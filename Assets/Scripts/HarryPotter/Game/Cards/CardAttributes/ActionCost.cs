using UnityEngine;

namespace HarryPotter.Game.Cards.CardAttributes
{
    public class ActionCost : CardAttribute
    {
        [Range(0, 2)]
        public int Amount;
    }
}