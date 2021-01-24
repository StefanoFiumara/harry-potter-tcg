using System;
using System.Collections.Generic;
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

        public int MinLessonCost;
        public int MaxLessonCost;

        public Alliance Ownership;

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
                Ownership = Ownership
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

            return targetSystem.GetTargetCandidates(owner, SearchQuery);
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, SearchQuery);

            // IMPORTANT: Check that there is at least one card targeted to prevent some possible silly misplays.
            //            Would the player ever actually want to play a card with no viable targets?
            return candidates.Count > 0;
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