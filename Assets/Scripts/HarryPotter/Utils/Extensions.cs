using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using UnityEngine;

namespace HarryPotter.Utils
{
    // TODO: Move non-generic methods out into their own extensions file
    public static class Extensions
    {
        public static Color WithAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));

        public static Sequence Move(this CardView cardView, Vector3 position, Vector3 rotation, float duration = 0.5f,
            float startDelay = 0f, float endDelay = 0f, bool doLocal = false)
            => DOTween.Sequence()
                .AppendInterval(startDelay)
                .Append(cardView.transform.DOMove(position, duration))
                .Join(cardView.transform.DORotate(rotation, duration))
                .AppendInterval(endDelay)
                ;
        
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src);

        public static TAttribute GetDataAttribute<TAttribute>(this CardData data)
        {
            var attribute = data.Attributes.OfType<TAttribute>().SingleOrDefault();
            return attribute;
        }
        
        public static TAttribute GetAttribute<TAttribute>(this Card card)
            where TAttribute : CardAttribute
        {
            var attribute = card.Attributes.OfType<TAttribute>().SingleOrDefault();
            return attribute;
        }
        
        public static List<TAttribute> GetAttributes<TAttribute>(this Card card)
            where TAttribute : CardAttribute
        {
            var attribute = card.Attributes.OfType<TAttribute>().ToList();
            return attribute;
        }

        public static List<Ability> GetAbilities(this Card card, AbilityType abilityType)
        {
            var abilities = card.Attributes.OfType<Ability>().Where(a => a.Type == abilityType).ToList();
            return abilities;
        }

        public static List<TSelector> GetTargetSelectors<TSelector>(this Card card, AbilityType abilityType) where TSelector : BaseTargetSelector
        {
            return card.GetAbilities(abilityType).Select(a => a.TargetSelector).OfType<TSelector>().ToList();
        }

        public static string GetFormattedTooltipText(this CardData data)
        {
            // TODO: flag to show current/max health for creatures
            var tooltipText = new StringBuilder();

            var lessonCost = data.GetDataAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount} {TextIcons.FromLesson(lessonCost.Type)}</align>");
            }
            
            tooltipText.AppendLine($"<b>{data.CardName}</b>");
            
            tooltipText.AppendLine($"<i>{data.Type}</i>");
            if (data.Tags != Tag.None)
            {
                tooltipText.AppendLine($"<size=10>{string.Join(" * ", data.Tags)}</size>");
            }
            

            var creature = data.GetDataAttribute<Creature>();
            if (creature != null)
            {
                tooltipText.AppendLine($"{TextIcons.ICON_ATTACK} {creature.Attack}");
                tooltipText.AppendLine($"{TextIcons.ICON_HEALTH} {creature.Health}");
            }
            
            if (!string.IsNullOrWhiteSpace(data.CardDescription))
            {
                tooltipText.AppendLine(data.TooltipText.Value);                
            }

            return tooltipText.ToString();
        }
        
        public static LessonType GetLessonType(this Card card)
        {
            var cost = card.GetAttribute<LessonCost>();
            if (cost != null)
            {
                return cost.Type;
            }

            var provider = card.GetAttribute<LessonProvider>();
            if (provider != null)
            {
                return provider.Type;
            }

            return LessonType.None;
        }

        public static HashSet<LessonType> GetLessonTypes(this IEnumerable<CardData> cards)
        {
            return cards
                .SelectMany(c => c.Attributes)
                .OfType<LessonProvider>()
                .Select(p => p.Type)
                .ToHashSet();
        }
        
        public static HashSet<LessonType> GetLessonTypes(this IEnumerable<Card> cards)
        {
            return cards
                .SelectMany(c => c.Attributes)
                .OfType<LessonProvider>()
                .Select(p => p.Type)
                .ToHashSet();
        }
        
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
    }
}