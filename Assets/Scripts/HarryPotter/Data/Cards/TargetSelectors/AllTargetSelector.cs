using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class AllTargetSelector : BaseTargetSelector
    {
        public int RequiredAmount;
        public Mark Mark;

        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, Mark);

            return candidates;
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, Mark);

            return candidates.Count >= RequiredAmount;
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<AllTargetSelector>();
            copy.Mark = Mark;
            return copy;
        }
    }
}