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

            // BUG: This implementation could damage a player multiple times in scenarios where
            //      the player is allowed to target more than one card and the enemy has multiple
            //      characters in play.
            foreach (var target in action.Targets)
            {
                if (target.Zone == Zones.Creatures)
                {
                    DamageCreature(action.SourceCard, target, action.Amount);
                }
                // TODO: Is there a better way to detect whether the action is meant for the enemy player?
                else if (target.Zone == Zones.Characters)
                {
                    // BUG: Will damage self if player clicks on own character, which some cards may not actually allow.
                    DamagePlayer(action.SourceCard, target.Owner, action.Amount);
                }
            }
        }

        private void OnPerformDamage(object sender, object args)
        {
            var action = (DamagePlayerAction) args;

            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            action.DiscardedCards = action.Target[Zones.Deck].Draw(action.Amount);

            for (var i = action.DiscardedCards.Count - 1; i >= 0; i--)
            {
                var card = action.DiscardedCards[i];
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
                action.IsLethal = true;
                var discardSystem = Container.GetSystem<DiscardSystem>();
                discardSystem.DiscardCard(action.SourceCard, action.Target, action);
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