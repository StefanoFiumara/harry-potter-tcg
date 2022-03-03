using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public static class CardExtensions
    {
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

        public static HashSet<LessonType> GetLessonProviderTypes(this IEnumerable<CardData> cards)
        {
            return cards
                .SelectMany(c => c.Attributes)
                .OfType<LessonProvider>()
                .Select(p => p.Type)
                .ToHashSet();
        }

        public static HashSet<LessonType> GetLessonProviderTypes(this IEnumerable<Card> cards)
        {
            return cards
                .SelectMany(c => c.Attributes)
                .OfType<LessonProvider>()
                .Select(p => p.Type)
                .ToHashSet();
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

        public static LessonType GetDataLessonType(this CardData card)
        {
            var cost = card.GetDataAttribute<LessonCost>();
            if (cost != null)
            {
                return cost.Type;
            }

            var provider = card.GetDataAttribute<LessonProvider>();
            if (provider != null)
            {
                return provider.Type;
            }

            return LessonType.None;
        }

        public static void Highlight(this IEnumerable<CardView> cards, Color color)
        {
            foreach (var cardView in cards)
            {
                cardView.Highlight(color);
            }
        }

        public static void ClearTargetCounters(this IEnumerable<CardView> cards)
        {
            foreach (var cardView in cards)
            {
                cardView.HideTargetCounter();
            }
        }

        public static IOrderedEnumerable<CardData> SortCards(this IEnumerable<CardData> cards)
        {
            return cards.OrderBy(c => c.GetDataAttribute<LessonCost>()?.Type)
                .ThenBy(c => c.Type)
                .ThenBy(c => c.GetDataAttribute<LessonCost>()?.Amount)
                .ThenBy(c => c.CardName);
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

            var provider = data.GetDataAttribute<LessonProvider>();
            if (provider != null)
            {
                var icon = TextIcons.FromLesson(provider.Type);
                var icons = string.Join(" ", Enumerable.Repeat(icon, provider.Amount));
                tooltipText.AppendLine($"\nProvides {icons}");
            }

            return tooltipText.ToString();
        }
    }
}
