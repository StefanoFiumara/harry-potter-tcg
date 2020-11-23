using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Utils
{
    public static class Colors
    {
        public static readonly Color TargetCandidate = new Color(0.61f, 1f, 0.99f);
        public static readonly Color Targeted = new Color(1f, 0.45f, 0.44f);
        public static readonly Color NeedsTargets = new Color(1f, 0.98f, 0.67f);
        public static readonly Color Active = new Color(0.61f, 1f, 0.64f);

        // TODO: Organize these colors better
        public static readonly Color COMC_Left = Color.white;
        public static readonly Color COMC_Right = new Color(0.82f, 0.45f, 0f);
        
        public static readonly Color Charms_Left = Color.white;
        public static readonly Color Charms_Right = Color.blue;

        public static readonly Color Transfiguration_Left = Color.white;
        public static readonly Color Transfiguration_Right = Color.red;
        
        public static (Color Left, Color Right) GetLessonColor(LessonType lesson)
        {
            switch (lesson)
            {
                case LessonType.Creatures:
                    return (COMC_Left, COMC_Right);
                case LessonType.Charms:
                    return (Charms_Left, Charms_Right);
                case LessonType.Transfiguration:
                    return (Transfiguration_Left, Transfiguration_Right);
                default:
                    return (Color.white, Color.white);
            }
        }
        
    }
}