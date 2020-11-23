using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    [Serializable]
    public class RandomTargetSelector : BaseTargetSelector
    {
        public Mark Mark;
        public int Amount;
        
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            var targetSystem = game.GetSystem<TargetSystem>();

            var candidates = targetSystem.GetTargetCandidates(owner, Mark);

            return candidates.TakeRandom(Amount);
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