using UnityEngine;

namespace HarryPotter.Data.Cards
{
    public abstract class CardAttribute : ScriptableObject
    {
        /// <summary>
        /// Initializes the Attribute in order to cache all default values of the object.
        /// </summary>
        public abstract void InitAttribute();
        
        // TODO: This needs to be called from somewhere to reset attributes back to their default state.
        /// <summary>
        /// Resets any modified attributes back to their default state
        /// </summary>
        public abstract void ResetAttribute();
    }
}