using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class ManualTargetSelector : BaseTargetSelector
    {
        public override List<Card> SelectTargets(IContainer game)
        {
            var target = Owner.GetAttribute<RequireTarget>();
            return target.Selected.ToList();
        }
    }
}