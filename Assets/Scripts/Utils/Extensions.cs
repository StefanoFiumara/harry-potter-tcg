using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Cards.CardAttributes;
using HarryPotter.Input;
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

        public static bool MeetsConditions(this List<PlayCondition> list, GameState gs)
        {
            return list.Count > 0
                   && list.All(c => c.MeetsCondition(gs.LocalPlayer, gs.EnemyPlayer));
        }

        public static bool IsInPlay(this Zone z) 
            => new[]{
            Zone.Items,
            Zone.Creatures,
            Zone.Location,
            Zone.Match,
            Zone.Characters,
            Zone.Adventure,
            Zone.Lessons
        }.Contains(z);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src);

        public static TargetRequirement GetTargetRequirement(this CardView card, TargetingType targetType)
        {
            switch (targetType)
            {
                case TargetingType.Hand:
                    return card.GetCardAttribute<FromHandTargetRequirement>();

                case TargetingType.Effect:
                    return card.GetCardAttribute<FromPlayTargetRequirement>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }

        public static T GetCardAttribute<T>(this CardView card) where T : CardAttribute
        {
            return card.Data.Attributes.OfType<T>().SingleOrDefault();
        }
    }
}