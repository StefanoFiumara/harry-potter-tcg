using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class DamageSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<DamagePlayerOrCreatureAction>(), OnPerformDamagePlayerOrCreature);
            Global.Events.Subscribe(Notification.Perform<DamageCreatureAction>(), OnPerformDamageCreature);
            Global.Events.Subscribe(Notification.Perform<DamagePlayerAction>(), OnPerformDamage);
            
        }

        public void DamagePlayer(Card source, Player target, int amount)
        {
            var action = new DamagePlayerAction(source, target, amount);
            
            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
            }
        }

        private void DamageCreature(Card source, Card target, int amount)
        {
            var action = new DamageCreatureAction(source, target, amount);
            
            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
            }
        }

        private void OnPerformDamagePlayerOrCreature(object sender, object args)
        {
            var action = (DamagePlayerOrCreatureAction) args;

            if (action.Target.Data.Type == CardType.Creature)
            {
                DamageCreature(action.Source, action.Target, action.Amount);
            }
            // TODO: Is there a better way to detect whether the action is meant for the enemy player?
            else if (action.Target.Data.Type == CardType.Character)
            {
                DamagePlayer(action.Source, action.Target.Owner, action.Amount);
            }
        }

        private void OnPerformDamage(object sender, object args)
        {
            var action = (DamagePlayerAction) args;

            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            action.DiscardedCards = action.Target[Zones.Deck].Draw(action.Amount);
            
            foreach (var card in action.DiscardedCards)
            {
                playerSystem.ChangeZone(card, Zones.Discard);
            }
        }
        
        private void OnPerformDamageCreature(object sender, object args)
        {
            var action = (DamageCreatureAction) args;

            var creatureStats = action.Target.GetAttribute<Creature>();

            if (creatureStats == null)
            {
                Debug.LogError("Attempted to call DamageCreature on a card without a Creature attribute!");
                return;
            }

            creatureStats.Health -= action.Amount;

            if (creatureStats.Health <= 0)
            {
                // TODO: Research how the reaper phase of the ActionSystem plays into this...It might not be necessary for HPTCG.
                var discardSystem = Container.GetSystem<DiscardSystem>();
                discardSystem.DiscardCard(action.Source, action.Target);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<DamagePlayerOrCreatureAction>(), OnPerformDamagePlayerOrCreature);
            Global.Events.Unsubscribe(Notification.Perform<DamageCreatureAction>(), OnPerformDamageCreature);
            Global.Events.Unsubscribe(Notification.Perform<DamagePlayerAction>(), OnPerformDamage);
            
        }
    }
}