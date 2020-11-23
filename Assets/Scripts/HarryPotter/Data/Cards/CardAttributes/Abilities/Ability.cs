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

        public ActionDefinition()
        {
            ActionName = string.Empty;
            Params = string.Empty;
        }
    }
    
    public class Ability : CardAttribute
    {
        [HideInInspector]
        public List<ActionDefinition> Actions = new List<ActionDefinition>();
        
        [HideInInspector]
        public BaseTargetSelector TargetSelector;
        
        public AbilityType Type;

        public override void InitAttribute()
        {
            
        }

        public override void ResetAttribute()
        {
            
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<Ability>();
            copy.Actions = Actions;
            copy.Type = Type;

            if (TargetSelector != null)
            {
                copy.TargetSelector = TargetSelector.Clone();
            }
            
            copy.InitAttribute();
            return copy;
        }

        public string GetParams(string actionName)
        {
            return Actions.SingleOrDefault(a => a.ActionName == actionName)?.Params ?? string.Empty;
        }
    }
}