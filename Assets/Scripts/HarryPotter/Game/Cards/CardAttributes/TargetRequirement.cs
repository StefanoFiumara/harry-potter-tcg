using System.Collections.Generic;
using HarryPotter.Enums;

namespace HarryPotter.Game.Cards.CardAttributes
{
    public class TargetRequirement : CardAttribute
    {
        // TODO: prettify editor for this attribute
        public int NumberOfTargets;
        public bool UpTo;

        public List<CardType> TargetableTypes;

        public bool CanTargetPlayer;

        public bool CanTargetEnemy;
        public bool CanTargetOwn;

        public bool HasEnoughTargets(int targetCount)
        {
            if (UpTo) return targetCount > 0 && targetCount <= NumberOfTargets;

            return targetCount == NumberOfTargets;
        }

        
    }

    public class FromHandTargetRequirement : TargetRequirement { }
    public class FromPlayTargetRequirement : TargetRequirement { }
}