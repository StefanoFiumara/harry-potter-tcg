using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public abstract class CardAttribute : ScriptableObject
    {
        public Card Owner { get; set; }
        
        /// <summary>
        /// Initializes the Attribute in order to cache all default values of the object.
        /// </summary>
        public abstract void InitAttribute();
        
        /// <summary>
        /// Resets any modified attributes back to their default state
        /// </summary>
        public abstract void ResetAttribute();

        public abstract CardAttribute Clone();
    }
}