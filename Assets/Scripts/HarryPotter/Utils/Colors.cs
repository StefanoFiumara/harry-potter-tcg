using UnityEngine;

namespace HarryPotter.Utils
{
    public static class Colors
    {
        // TODO: Read these in from GameSettings Asset?
        // Target System Colors
        public static readonly Color IsTargetCandidate = new Color(0.48f, 0.76f, 1f);
        public static readonly Color IsTargeted        = new Color(1f, 0.88f, 0.02f);
        public static readonly Color NeedsTargets      = new Color(1f, 0.44f, 0.12f);
        public static readonly Color HasTargets        = new Color(0.42f, 1f, 0.29f);

        // Lesson Colors
        public static readonly Color Creatures       = new Color(1f, 0.53f, 0.14f);
        public static readonly Color Charms          = new Color(0.1f, 0.17f, 1f);
        public static readonly Color Transfiguration = new Color(1f, 0.14f, 0.25f);
        public static readonly Color Potions         = new Color(0.18f, 0.78f, 0.08f);
        public static readonly Color Quidditch       = new Color(1f, 0.93f, 0.18f);
        
        
        // Highlight Colors
        public static readonly Color Unplayable = Color.red;
        public static readonly Color Playable   = new Color(0.15f, 1f, 0.22f);
        public static readonly Color Activatable = new Color(0.53f, 1f, 0.89f);
        
        public static Color WithAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));
    }
}