using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    //TODO: Some cards can adjust these values, but their original values must remain...
    public class LessonCost
    {
        [Range(0, 20)]
        public int Amount;
        public LessonType Type;
    }
}