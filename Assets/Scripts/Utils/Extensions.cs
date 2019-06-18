using DG.Tweening;
using HarryPotter.Game.Cards;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static Color WithAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));

        public static Sequence Move(this CardView card, Vector3 position, Vector3 rotation, float duration = 0.5f) 
            => DOTween.Sequence()
                .Append(card.transform.DOMove(position, duration))
                .Join(card.transform.DORotate(rotation, duration));
    }
}