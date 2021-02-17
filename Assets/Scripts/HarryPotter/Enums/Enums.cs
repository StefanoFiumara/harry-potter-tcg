using System;

namespace HarryPotter.Enums
{
    [Flags]
    public enum Zones
    {
        None       = 0,
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
        None       = 0,
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
        None        = 0,
        Unique      = 1,
        Healing     = 1 << 1,
        Wand        = 1 << 2,
        Cauldron    = 1 << 3,
        Broom       = 1 << 4,
        Plant       = 1 << 5,
        Owl         = 1 << 6,
        Witch       = 1 << 7,
        Wizard      = 1 << 8,
        Gryffindor  = 1 << 9,
        Slytherin   = 1 << 10,
        Hufflepuff  = 1 << 11,
        Ravenclaw   = 1 << 12,
        Spider      = 1 << 13,
        Book        = 1 << 14
    }

    [Flags]
    public enum LessonType
    {
        None            = 0,
        Creatures       = 1, 
        Charms          = 1 << 1, 
        Transfiguration = 1 << 2, 
        Potions         = 1 << 3, 
        Quidditch       = 1 << 4,
        Any             = Creatures | Charms | Transfiguration | Potions | Quidditch
    }

    public enum ControlMode
    {
        Local    = 0,
        Computer = 1,
        Remote   = 2
    }

    [Flags]
    public enum Alliance
    {
        None = 0,
        Ally = 1 << 0,
        Enemy = 1 << 1,
        Any = Ally | Enemy
    }
    
    public enum AbilityType
    {
        PlayEffect,
        OnTurnStart,
        OnTurnEnd,
        ActivateEffect,
        PlayCondition,
        ActivateCondition
    }

    public enum PreviewSortOrder
    {
        Original,
        ByType,
        Shuffle
    }
}