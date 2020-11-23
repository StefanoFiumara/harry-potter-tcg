using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class AllTargetSelector : BaseTargetSelector
    {
        public Mark Mark;

        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            throw new NotImplementedException();
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<AllTargetSelector>();
            copy.Mark = Mark;
            return copy;
        }
    }
}