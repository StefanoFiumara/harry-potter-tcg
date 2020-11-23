using System.Collections.Generic;
using HarryPotter.UI;

namespace HarryPotter.Enums
{
    public static class EnumExtensions
    {
        private static readonly Dictionary<CardType, Zones> ZoneTypeMap = new Dictionary<CardType, Zones>
        {
            { CardType.Lesson,     Zones.Lessons    },
            { CardType.Creature,   Zones.Creatures  },
            { CardType.Spell,      Zones.Discard    },
            { CardType.Item,       Zones.Items      },
            { CardType.Location,   Zones.Location   },
            { CardType.Match,      Zones.Match      },
            { CardType.Adventure,  Zones.Adventure  },
            { CardType.Character,  Zones.Characters }
        };
        
        public static Zones ToTargetZone(this CardType type) => ZoneTypeMap[type];

        private const Zones BOARD_ZONES =   Zones.Characters 
                                          | Zones.Lessons 
                                          | Zones.Creatures 
                                          | Zones.Items 
                                          | Zones.Location 
                                          | Zones.Match 
                                          | Zones.Adventure;

        public static bool IsInBoard(this Zones zone)
        {
            return (BOARD_ZONES & zone) != 0;
        }
        
        public static bool HasTag(this Tag tags, Tag tag)
        {
            return (tags & tag) != 0;
        }

        private const CardType HORIZONTAL_TYPES =   CardType.Lesson
                                                  | CardType.Creature
                                                  | CardType.Item
                                                  | CardType.Location
                                                  | CardType.Match
                                                  | CardType.Adventure
                                                  | CardType.Character;
        public static bool IsHorizontal(this CardType type)
        {
            return (HORIZONTAL_TYPES & type) != 0;
        }

        public static bool HasAlliance(this Alliance source, Alliance target)
        {
            return (source & target) == source;
        }
        
        public static bool HasZone(this Zones source, Zones target)
        {
            return (source & target) == source;
        }
        
        public static bool HasCardType(this CardType source, CardType target)
        {
            return (source & target) == source;
        }

        public static bool HasLessonType(this LessonType source, LessonType target)
        {
            return (source & target) == source;
        }
    }
}