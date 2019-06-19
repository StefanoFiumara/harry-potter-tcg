using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HarryPotter.Game.Cards
{
    public abstract class CardAction : ScriptableObject
    {
        public abstract Sequence Execute(CardView card, List<CardView> targets);
    }
}