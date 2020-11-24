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
        public static readonly Color COMC_Left = new Color(1f, 0.62f, 0.18f);
        public static readonly Color COMC_Right = new Color(0.47f, 0.27f, 0.13f);
        
        public static readonly Color Charms_Left = Color.white;
        public static readonly Color Charms_Right = Color.blue;

        public static readonly Color Transfiguration_Left = Color.white;
        public static readonly Color Transfiguration_Right = new Color(1f, 0.27f, 0.51f);
        
        public static readonly Color Quidditch_Left = Color.white;
        public static readonly Color Quidditch_Right = new Color(1f, 0.93f, 0.18f);
        
        public static readonly Color Potions_Left = Color.white;
        public static readonly Color Potions_Right = new Color(0.18f, 0.78f, 0.08f);
        
        public static (Color Left, Color Right) GetLessonColorGradient(LessonType lesson)
        {
            switch (lesson)
            {
                case LessonType.Creatures:
                    return (COMC_Left, COMC_Right);
                case LessonType.Charms:
                    return (Charms_Left, Charms_Right);
                case LessonType.Transfiguration:
                    return (Transfiguration_Left, Transfiguration_Right);
                case LessonType.Quidditch:
                    return (Quidditch_Left, Quidditch_Right);
                case LessonType.Potions:
                    return (Potions_Left, Potions_Right);
                default:
                    return (Color.white, Color.white);
            }
        }
        
    }
}