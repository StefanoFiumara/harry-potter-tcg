using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HarryPotter.Game.Cards
{
    public abstract class PlayAction : ScriptableObject
    {
        public abstract Sequence Execute(CardView card, List<CardView> targets);
    }
}