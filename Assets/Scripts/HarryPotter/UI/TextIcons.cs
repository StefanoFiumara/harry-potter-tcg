using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.UI
{
    public static class TextIcons
    {
        public static readonly Dictionary<LessonType, string> LessonIconMap = new Dictionary<LessonType, string>
        {
            { LessonType.Creatures,       TextIcons.ICON_CREATURES       },
            { LessonType.Charms,          TextIcons.ICON_CHARMS          },
            { LessonType.Transfiguration, TextIcons.ICON_TRANSFIGURATION },
            { LessonType.Potions,         TextIcons.ICON_POTIONS         },
            { LessonType.Quidditch,       TextIcons.ICON_QUIDDITCH       },
        };
        
        public const string ICON_CREATURES = @"<sprite name=""icon_creatures"">";
        public const string ICON_CHARMS = @"<sprite name=""icon_charms"">";
        public const string ICON_TRANSFIGURATION = @"<sprite name=""icon_transfiguration"">";
        public const string ICON_POTIONS = @"<sprite name=""icon_potions"">";
        public const string ICON_QUIDDITCH = @"<sprite name=""icon_quidditch"">";
    }
}