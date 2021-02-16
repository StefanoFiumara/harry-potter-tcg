using System.Collections.Generic;
using HarryPotter.Systems.Core;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class SelfTargetSelector : BaseTargetSelector
    {
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            return new List<Card> { owner };
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            return true;
        }

        public override BaseTargetSelector Clone()
        {
            return CreateInstance<SelfTargetSelector>();
        }
    }
}