using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class AllTargetSelector : BaseTargetSelector
    {
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

            // NOTE: Check that there is at least one card targeted to prevent some possible silly misplays.
            //       Would the player ever actually want to play a card with no viable targets?
            return candidates.Count > 0;
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<AllTargetSelector>();
            copy.Mark = Mark;
            return copy;
        }
    }
}