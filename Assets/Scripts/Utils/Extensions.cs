using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Enums;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static Color WithAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));

        public static Sequence Move(this Transform t, Vector3 position, Vector3 rotation, float duration = 0.5f) 
            => DOTween.Sequence()
                .Append(t.DOMove(position, duration))
                .Join(t.DORotate(rotation, duration));

        public static bool IsInPlay(this ZoneType z) 
            => new[]{
            ZoneType.Items,
            ZoneType.Creatures,
            ZoneType.Location,
            ZoneType.Match,
            ZoneType.Characters,
            ZoneType.Adventure,
            ZoneType.Lessons
        }.Contains(z);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src);
    }
}