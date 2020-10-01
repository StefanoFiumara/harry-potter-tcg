using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class AllTargetSelector : BaseTargetSelector
    {
        public Mark Mark;

        public override List<Card> SelectTargets(IContainer game)
        {
            throw new NotImplementedException();
        }
    }
}