using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Game.Cards.CardAttributes
{
    public class LessonProvider : CardAttribute // ILessonProvider interface? 
    {
        [Range(0, 10)]
        public int Amount;
        public LessonType Type;
    }
}