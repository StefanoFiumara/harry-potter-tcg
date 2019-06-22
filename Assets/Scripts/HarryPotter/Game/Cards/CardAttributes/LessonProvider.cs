using HarryPotter.Enums;

namespace HarryPotter.Game.Cards.CardAttributes
{
    public class LessonProvider : CardAttribute // ILessonProvider interface? 
    {
        public int Amount;
        public LessonType Type;
    }
}