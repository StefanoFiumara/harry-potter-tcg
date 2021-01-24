using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class ManualTargetSelector : BaseTargetSelector
    {
        public Mark Allowed;

        public int RequiredAmount;
        public int MaxAmount;
        
        public List<Card> Selected { get; set; }
        
        public override List<Card> SelectTargets(IContainer game, Card owner)
        {
            return Selected.ToList();
        }

        public override bool HasEnoughTargets(IContainer game, Card owner)
        {
            return Selected.Count >= RequiredAmount;
        }

        public override void InitSelector()
        {
            Selected = new List<Card>();
        }

        public override void ResetSelector()
        {
            Selected = new List<Card>();
        }

        public override BaseTargetSelector Clone()
        {
            var copy = CreateInstance<ManualTargetSelector>();
            copy.Allowed = Allowed;
            copy.RequiredAmount = RequiredAmount;
            copy.MaxAmount = MaxAmount;
            return copy;
        }
    }
}