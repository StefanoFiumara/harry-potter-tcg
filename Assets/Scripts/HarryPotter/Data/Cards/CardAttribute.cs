using UnityEngine;

namespace HarryPotter.Data.Cards
{
    public abstract class CardAttribute : ScriptableObject
    {
        /// <summary>
        /// Resets any modified attributes back to their default state
        /// </summary>
        public abstract void ResetAttribute();
    }
}