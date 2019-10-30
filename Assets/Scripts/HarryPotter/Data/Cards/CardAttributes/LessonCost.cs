using System;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    //TODO: Some cards can adjust these values, but their original values must remain...
    public class LessonCost : CardAttribute
    {
        [Range(0, 20)]
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