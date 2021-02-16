using UnityEngine;

namespace HarryPotter.Utils
{
    public static class Colors
    {
        // Target System Colors
        public static readonly Color IsTargetCandidate = new Color(0.34f, 1f, 0.86f);
        public static readonly Color IsTargeted        = new Color(1f, 0.88f, 0.02f);
        public static readonly Color NeedsTargets      = new Color(1f, 0.51f, 0.35f);
        public static readonly Color HasTargets        = new Color(0.42f, 1f, 0.29f);

        // Lesson Colors
        public static readonly Color Creatures       = new Color(1f, 0.53f, 0.14f);
        public static readonly Color Charms          = new Color(0.1f, 0.17f, 1f);
        public static readonly Color Transfiguration = new Color(1f, 0.14f, 0.25f);
        public static readonly Color Potions         = new Color(0.18f, 0.78f, 0.08f);
        public static readonly Color Quidditch       = new Color(1f, 0.93f, 0.18f);
        
        
        // Highlight Colors
        public static readonly Color Unplayable = new Color(1f, 0.07f, 0.13f);
        public static readonly Color Playable   = new Color(0.75f, 1f, 0.79f);
        public static readonly Color Activatable   = new Color(0.34f, 1f, 0.99f);
    }
}