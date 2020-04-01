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
        
        public static bool IsInBoard(this Zones zone)
        {
            return (Zones.Board & zone) != 0;
        }
        
        public static bool HasTag(this Tag tags, Tag tag)
        {
            return (tags & tag) != 0;
        }
    }
}