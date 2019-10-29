using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Game.Cards.CardAttributes
{
    public class LessonCost
    {
        [Range(0, 20)]
        public int Amount;
        public LessonType Type;
    }
}