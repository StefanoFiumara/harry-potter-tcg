using System;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class AbilitySystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<AbilityAction>(), OnPerformAbilityAction);
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