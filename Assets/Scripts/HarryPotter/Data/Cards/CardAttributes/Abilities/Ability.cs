using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes.Abilities
{
    [Serializable]
    public class ActionDefinition
    {
        public string ActionName;
        public string Params;
    }
    
    public class Ability : CardAttribute
    {
        [HideInInspector]
        public List<ActionDefinition> Actions = new List<ActionDefinition>();
        
        // TODO: Custom Inspector for this
        [HideInInspector]
        public BaseTargetSelector TargetSelector;
        
        public AbilityType Type;

        public override void InitAttribute()
        {
            if (TargetSelector != null)
            {
                TargetSelector.Owner = Owner;                
            }
        }

        public override void ResetAttribute()
        {
            
        }

        public string GetParams(string actionName)
        {
            return Actions.SingleOrDefault(a => a.ActionName == actionName)?.Params ?? string.Empty;
        }
    }
}