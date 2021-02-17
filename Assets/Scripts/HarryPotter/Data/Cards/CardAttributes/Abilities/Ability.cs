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

        public bool OncePerGameAbility;
        
        public bool IsActive { get; set; }

        public override void InitAttribute()
        {
            if (TargetSelector != null)
            {
                TargetSelector.InitSelector();
            }

            IsActive = true;
        }

        public override void ResetAttribute()
        {
            if (TargetSelector != null)
            {
                TargetSelector.ResetSelector();
            }

            IsActive = true;
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<Ability>();
            copy.Actions = Actions;
            copy.Type = Type;
            copy.OncePerGameAbility = OncePerGameAbility;

            if (TargetSelector != null)
            {
                copy.TargetSelector = TargetSelector.Clone();
            }
            
            copy.InitAttribute();
            return copy;
        }

        public override string ToString()
        {
            return $"Ability - {Type}: {string.Join(", ", Actions.Select(a => a.ActionName))}"; 
        }

        public string GetParams(string actionName)
        {
            return Actions.SingleOrDefault(a => a.ActionName == actionName)?.Params ?? string.Empty;
        }
    }
}