using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class ManualTargetSelector : BaseTargetSelector
    {
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            var target = owner.GetAttribute<ManualTarget>();
            return target.Selected.ToList();
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            var target = owner.GetAttribute<ManualTarget>();
            return target.Selected.Count >= target.RequiredAmount;
        }

        public override BaseTargetSelector Clone()
        {
            return CreateInstance<ManualTargetSelector>();
        }
    }
}