using System;
using System.Collections.Generic;

namespace HarryPotter.Enums
{
    public enum Zones
    {
        Deck,
        Discard,
        Hand,
        Characters,
        Lessons,
        Creatures,
        Location,
        Match,
        Items,
        Adventure
    }
    
    public enum CardType
    {
        Lesson, 
        Creature, 
        Spell, 
        Item, 
        Location, 
        Match, 
        Adventure, 
        Character
    }
    
    [Flags]
    public enum Tag
    {
        Unique      = 1,
        Healing     = 1 << 1,
        Wand        = 1 << 2,
        Cauldron    = 1 << 3,
        Broom       = 1 << 4,
        Plant       = 1 << 5,
        Owl         = 1 << 6
    }
    
    public enum LessonType
    {
        Creatures, 
        Charms, 
        Transfiguration, 
        Potions, 
        Quidditch
    }

    public enum ControlMode
    {
        Local,
        Computer,
        Remote
    }

    public static class EnumExtensions
    {
        private static readonly Dictionary<CardType, Zones> ZoneTypeMap = new Dictionary<CardType, Zones>
        {
            { CardType.Lesson,     Zones.Lessons },
            { CardType.Creature,   Zones.Creatures },
            { CardType.Spell,      Zones.Discard },
            { CardType.Item,       Zones.Items },
            { CardType.Location,   Zones.Location },
            { CardType.Match,      Zones.Match },
            { CardType.Adventure,  Zones.Adventure },
            { CardType.Character,  Zones.Characters }
        };
        public static Zones ToTargetZone(this CardType type) => ZoneTypeMap[type];
    }
}