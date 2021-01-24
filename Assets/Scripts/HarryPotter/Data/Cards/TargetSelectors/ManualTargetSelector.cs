using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class ManualTargetSelector : BaseTargetSelector
    {
        public ManualTarget ManualTarget;
        
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            return ManualTarget.Selected.ToList();
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            return ManualTarget.Selected.Count >= ManualTarget.RequiredAmount;
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<ManualTargetSelector>();
            copy.ManualTarget = (ManualTarget) ManualTarget.Clone();
            return copy;
        }
    }
}