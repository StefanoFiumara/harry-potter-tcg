using System;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    [Serializable]
    public class Mark
    {
        public Alliance Alliance;
        public Zones Zones;
        public CardType CardType;
        public LessonType LessonType;
        public Tag RestrictedTags;
    }
}