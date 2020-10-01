using System.Collections.Generic;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Data.Cards.TargetSelectors
{
    public abstract class BaseTargetSelector : ScriptableObject
    {
        public Card Owner { get; set; }
        
        public abstract List<Card> SelectTargets(IContainer game);
    }
}