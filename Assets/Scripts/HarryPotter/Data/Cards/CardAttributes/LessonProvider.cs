using System;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class LessonProvider : CardAttribute 
    {
        public int Amount;
        public LessonType Type;

        [HideInInspector] public int DefaultAmount;

        private void Awake()
        {
            DefaultAmount = Amount;
        }

        public override void ResetAttribute()
        {
            Amount = DefaultAmount;
        }
    }
}