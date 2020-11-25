using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Utils
{
    public static class Colors
    {
        // Target System Colors
        public static readonly Color IsTargetCandidate = new Color(0.34f, 1f, 0.86f);
        public static readonly Color IsTargeted        = new Color(1f, 0.88f, 0.02f);
        public static readonly Color NeedsTargets      = new Color(1f, 0.48f, 0.27f);
        public static readonly Color HasTargets        = new Color(0.29f, 1f, 0.31f);

        // Lesson Colors
        public static readonly Color Creatures       = new Color(1f, 0.62f, 0.18f);
        public static readonly Color Charms          = new Color(0f, 0f, 1f);
        public static readonly Color Transfiguration = new Color(1f, 0.27f, 0.51f);
        public static readonly Color Quidditch       = new Color(1f, 0.93f, 0.18f);
        public static readonly Color Potions         = new Color(0.18f, 0.78f, 0.08f);
        
        // Highlight Colors
        public static readonly Color Unplayable = new Color(1f, 0f, 0f);
        public static readonly Color Playable   = new Color(0f, 1f, 0f);
    }
}