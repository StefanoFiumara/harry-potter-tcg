using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class SpellSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Subscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Subscribe(Notification.Perform<CastSpellAction>(), OnPerformCastSpell);
        }

        //TODO: OnValidatePlayCard - if the card is a spell and it has a target selector, make sure there are targets available (?)
        
        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.Card.Data.Type == CardType.Spell)
            {
                var spellAction = new CastSpellAction(action.Card);
                Container.AddReaction(spellAction);
            }
        }

        private void OnPrepareCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            var ability = action.Card.GetAttribute<Ability>();

            if (ability == null || ability.Type != AbilityType.WhenPlayed)
            {
                Debug.LogWarning($"CastSpellAction - No ability found for card {action.Card.Data.CardName}");
                return;
            }
            
            var reaction = new AbilityAction(ability);
            Container.AddReaction(reaction);
        }

        private void OnPerformCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            playerSystem.ChangeZone(action.Card, Zones.Discard);
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPerformCastSpell);
        }
    }
}