using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
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

            var abilities = action.SourceCard.GetAbilities(AbilityType.ActivateEffect);

            if (abilities.Count == 0)
            {
                validator.Invalidate($"Card does not have effect to activate.");
            }

            if (abilities.Any(a => a.OncePerGameAbility && !a.IsActive))
            {
                validator.Invalidate($"Once per game ability has already been used.");
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

                action.Ability.IsActive = false;
                Container.AddReaction(actionInstance);
            }
        }

        public void TriggerAbility(Card card, AbilityType type)
        {
            var abilities = card.GetAbilities(type);

            foreach (var ability in abilities)
            {
                var action = new AbilityAction(ability);
                if (Container.GetSystem<ActionSystem>().IsActive)
                {
                    Container.AddReaction(action);
                }
                else
                {
                    Container.Perform(action);
                }
            }
        }

        public void TriggerAbilities(List<Card> cards, AbilityType type)
        {
            foreach (var card in cards)
            {
                TriggerAbility(card, type);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<AbilityAction>(), OnPerformAbilityAction);
            Global.Events.Unsubscribe(Notification.Validate<ActivateCardAction>(), OnValidateActivateCard);
        }
    }
}
