using System.Collections.Generic;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public abstract class BaseTargetSelector : ScriptableObject
    {
        public abstract List<Card> SelectTargets(IContainer game, Card owner);

        public abstract bool HasEnoughTargets(IContainer game, Card owner);
        
        public abstract BaseTargetSelector Clone();
    }
}