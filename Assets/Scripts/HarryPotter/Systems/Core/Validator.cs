using System.Collections.Generic;
using HarryPotter.GameActions;
using UnityEngine;

namespace HarryPotter.Systems.Core
{
    public class Validator
    {
        public bool IsValid { get; private set; }

        public List<string> ValidationErrors { get; private set; }
        
        public Validator()
        {
            IsValid = true;
            ValidationErrors = new List<string>();
        }

        public void Invalidate(string reason)
        {
            ValidationErrors.Add(reason);
            IsValid = false;
        }
    }

    public static class ValidatorExtensions
    {
        public static Validator Validate(this GameAction action)
        {
            var validator = new Validator();
            var eventName = Notification.Validate(action.GetType());
            
            Global.Events.Publish(eventName, validator, action);

            return validator;
        }
    }
}