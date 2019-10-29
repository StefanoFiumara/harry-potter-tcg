using System;

namespace HarryPotter.Enums
{
    public enum ZoneType
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
        Unique      = 1 << 0,
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

    public enum ActionType
    {
        DrawCard,
        CardFromHand,
        InPlayCardEffect,
        InPlayCardReaction
    }
}