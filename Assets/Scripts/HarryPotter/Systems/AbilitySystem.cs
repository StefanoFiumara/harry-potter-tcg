using System;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class AbilitySystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<AbilityAction>(), OnPerformAbilityAction);
            Global.Events.Subscribe(Notification.Validate<ActivateCardAction>(), OnValidateActivateCard);
        }

        private void OnValidateActivateCard(object sender, object args)
        {
            var action = (ActivateCardAction) sender;
            var validator = (Validator) args;

            var abilities = action.Card.GetAbilities(AbilityType.ActivateEffect);

            if (abilities.Count == 0)
            {
                validator.Invalidate($"Card does not have effect to activate.");
            }
        }
        
        private void OnPerformAbilityAction(object sender, object args)
        {
            var action = (AbilityAction) args;

            int actionPriority = 99;
            
            foreach (var actionDef in action.Ability.Actions)
            {
                var actionType = Type.GetType($"HarryPotter.GameActions.Actions.{actionDef.ActionName}");

                if (actionType == null)
                {
                    Debug.LogError($"Ability System could not find action name: {actionDef.ActionName}");
                    return;
                }
                var actionInstance = (GameAction) Activator.CreateInstance(actionType);

                var loader = actionInstance as IAbilityLoader;
                loader?.Load(Container, action.Ability);

                actionInstance.Priority = actionPriority--;
            
                Container.AddReaction(actionInstance);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<AbilityAction>(), OnPerformAbilityAction);
        }
    }
}