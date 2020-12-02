using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.Views.UI
{
    public static class TextIcons
    {
        public static readonly Dictionary<LessonType, string> LessonIconMap = new Dictionary<LessonType, string>
        {
            { LessonType.Creatures,       ICON_CREATURES       },
            { LessonType.Charms,          ICON_CHARMS          },
            { LessonType.Transfiguration, ICON_TRANSFIGURATION },
            { LessonType.Potions,         ICON_POTIONS         },
            { LessonType.Quidditch,       ICON_QUIDDITCH       },
        };
        
        public const string ICON_CREATURES = @"<sprite name=""lesson-comc"">";
        public const string ICON_CHARMS = @"<sprite name=""lesson-charms"">";
        public const string ICON_TRANSFIGURATION = @"<sprite name=""lesson-trans"">";
        public const string ICON_POTIONS = @"<sprite name=""lesson-potions"">";
        public const string ICON_QUIDDITCH = @"<sprite name=""lesson-quidditch"">";
        
        public const string ICON_ACTIONS = @"<sprite name=""icon-actions"">";
        
        public const string MOUSE_LEFT = @"<sprite name=""mouse-left"">";
        public const string MOUSE_RIGHT = @"<sprite name=""mouse-right"">";
        
        public const string ICON_ATTACK = @"<sprite name=""icon-attack"">";
        public const string ICON_HEALTH = @"<sprite name=""icon-health"">";

        public static string FromLesson(LessonType lesson)
        {
            return LessonIconMap[lesson];
        }
    }
}