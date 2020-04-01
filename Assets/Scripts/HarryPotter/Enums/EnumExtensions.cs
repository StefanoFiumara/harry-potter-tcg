using System.Collections.Generic;

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
    }
}