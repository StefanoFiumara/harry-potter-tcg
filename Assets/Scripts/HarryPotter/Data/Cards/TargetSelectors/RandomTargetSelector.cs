using System;
using System.Collections.Generic;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    [Serializable]
    public class RandomTargetSelector : BaseTargetSelector
    {
        public Mark Mark;
        public int Amount; // TODO: Switch to Min/Max?
        
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, Mark);

            return candidates.TakeRandom(Amount);
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, Mark);

            return candidates.Count >= Amount;
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<RandomTargetSelector>();
            copy.Mark = Mark;
            copy.Amount = Amount;
            return copy;

        }
    }
}