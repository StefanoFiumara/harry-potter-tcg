using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    [Serializable]
    public class CardSearchQuery
    {
        // TODO: Custom editor for this?
        public string CardName;
        public CardType CardType;

        public LessonType LessonCostType;
        public LessonType LessonProviderType;

        public Alliance Ownership;
        public int MinLessonCost;
        public int MaxLessonCost;

        public Tag Tags;
        public Zones Zone;

        public CardSearchQuery Clone()
        {
            return new CardSearchQuery
            {
                CardName = CardName,
                CardType = CardType,
                LessonCostType = LessonCostType,
                LessonProviderType = LessonProviderType,
                MinLessonCost = MinLessonCost,
                MaxLessonCost = MaxLessonCost,
                Ownership = Ownership,
                Tags = Tags,
                Zone = Zone
            };
        }
    }

    public enum EnoughTargetsCondition
    {
        CardCount,
        LessonsProvided
    }
    
    public class AdvancedTargetSelector : BaseTargetSelector
    {
        public int RequiredAmount;
        public int MaxAmount;

        public CardSearchQuery SearchQuery;

        // TODO: Clean this up into a more general structure so that more target conditions can be added.
        public EnoughTargetsCondition EnoughTargetsCondition;
        
        public LessonType LessonsProvidedType;
        public int LessonsProvidedCount;
        
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            return targetSystem.GetTargetCandidates(owner, SearchQuery, MaxAmount);
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, SearchQuery, MaxAmount);

            switch (EnoughTargetsCondition)
            {
                case EnoughTargetsCondition.CardCount:
                    return candidates.Count >= RequiredAmount;
                case EnoughTargetsCondition.LessonsProvided:
                {
                    var lessonProviders = candidates.Select(c => c.GetAttribute<LessonProvider>()).Where(a => a != null);

                    return lessonProviders.Where(p => p.Type == LessonsProvidedType).Sum(p => p.Amount) >= LessonsProvidedCount;
                }
                default:
                    Debug.LogWarning("AdvancedTargetSelector's HasEnoughTargets method did not have a valid EnoughTargetsCondition!");
                    return false;
            }
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<AdvancedTargetSelector>();
            copy.RequiredAmount = RequiredAmount;
            copy.MaxAmount = MaxAmount;
            copy.SearchQuery = SearchQuery.Clone();
            copy.EnoughTargetsCondition = EnoughTargetsCondition;
            copy.LessonsProvidedType = LessonsProvidedType;
            copy.LessonsProvidedCount = LessonsProvidedCount;
            return copy;
        }
    }
}