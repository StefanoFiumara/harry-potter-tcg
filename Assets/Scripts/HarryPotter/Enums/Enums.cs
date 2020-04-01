using System;
using System.Collections.Generic;

namespace HarryPotter.Enums
{
    [Flags]
    public enum Zones
    {
        Deck       = 1,
        Discard    = 1 << 1,
        Hand       = 1 << 2,
        Characters = 1 << 3,
        Lessons    = 1 << 4,
        Creatures  = 1 << 5,
        Location   = 1 << 6,
        Match      = 1 << 7,
        Items      = 1 << 8,
        Adventure  = 1 << 9
    }

    [Flags]
    public enum CardType
    {
        Lesson     = 1, 
        Creature   = 1 << 1, 
        Spell      = 1 << 2, 
        Item       = 1 << 3, 
        Location   = 1 << 4, 
        Match      = 1 << 5, 
        Adventure  = 1 << 6, 
        Character  = 1 << 7
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
        Creatures       = 0, 
        Charms          = 1, 
        Transfiguration = 2, 
        Potions         = 3, 
        Quidditch       = 4
    }

    public enum ControlMode
    {
        Local    = 0,
        Computer = 1,
        Remote   = 2
    }
}