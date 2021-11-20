using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class SpellSystem : GameSystem, IAwake, IDestroy
    {
        private AbilitySystem _abilitySystem;

        public void Awake()
        {
            _abilitySystem = Container.GetSystem<AbilitySystem>();
            
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Subscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Subscribe(Notification.Perform<CastSpellAction>(), OnPerformCastSpell);
        }

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;
            var playerSystem = Container.GetSystem<PlayerSystem>();

            if (action.SourceCard.Data.Type == CardType.Spell)
            {
                playerSystem.ChangeZone(action.SourceCard, Zones.None);
                var spellAction = new CastSpellAction(action.SourceCard);
                Container.AddReaction(spellAction);
            }
        }

        private void OnPrepareCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            
            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.PlayCondition);
            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.PlayEffect);

        }

        private void OnPerformCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            var discardSystem = Container.GetSystem<DiscardSystem>();
            
            discardSystem.DiscardCard(action.SourceCard, action.SourceCard);
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPerformCastSpell);
        }
    }
}
