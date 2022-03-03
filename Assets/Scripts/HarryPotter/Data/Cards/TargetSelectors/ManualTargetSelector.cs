using System.Collections.Generic;
using System.Linq;
using HarryPotter.Systems.Core;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public class ManualTargetSelector : BaseTargetSelector
    {
        public Mark Allowed;

        public int RequiredAmount;
        public int MaxAmount;

        public string TargetPrompt;

        //TODO: Flag for the input system to determine if the enemy player is supposed to make the target selection

        public List<Card> Selected { get; set; }

        public string FormattedTargetPrompt =>
            (TargetPrompt ?? string.Empty)
                .Replace($"{{{nameof(RequiredAmount)}}}",$"{RequiredAmount}")
                .Replace($"{{{nameof(MaxAmount)}}}",$"{MaxAmount}");

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
