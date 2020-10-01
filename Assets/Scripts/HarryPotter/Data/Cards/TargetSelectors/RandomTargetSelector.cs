using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    [Serializable]
    public class RandomTargetSelector : BaseTargetSelector
    {
        public Mark Mark;
        public int Amount;
        
        public override List<Card> SelectTargets(IContainer game)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(Owner, Mark);

            return candidates.TakeRandom(Amount);
        }
    }
}