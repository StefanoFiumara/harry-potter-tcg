using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;

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
    
    public class AdvancedTargetSelector : BaseTargetSelector
    {
        public int RequiredAmount;
        public int MaxAmount;

        public CardSearchQuery SearchQuery;
        
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            return targetSystem.GetTargetCandidates(owner, SearchQuery, MaxAmount);
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, SearchQuery, MaxAmount);

            return candidates.Count >= RequiredAmount;
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<AdvancedTargetSelector>();
            copy.RequiredAmount = RequiredAmount;
            copy.MaxAmount = MaxAmount;
            copy.SearchQuery = SearchQuery.Clone();
            return copy;
        }
    }
}