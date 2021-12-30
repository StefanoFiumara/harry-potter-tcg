using System.Linq;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class AdventureSystem : GameSystem, IAwake, IDestroy
    {
        private AbilitySystem _abilitySystem;
        private DiscardSystem _discardSystem;

        public void Awake()
        {
            _abilitySystem = Container.GetSystem<AbilitySystem>();
            _discardSystem = Container.GetSystem<DiscardSystem>();

            Global.Events.Subscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
            Global.Events.Subscribe(Notification.Prepare<SolveAdventureAction>(), OnPrepareSolveAdventure);
        }

        private void OnPrepareSolveAdventure(object sender, object args)
        {
            var action = (SolveAdventureAction) args;

            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.AdventureSolveCondition);
            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.AdventureSolveEffect);
            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.AdventureReward);

            _discardSystem.DiscardCard(action.SourceCard, action.SourceCard, action);


        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;

            if (action.SourceCard.Data.Type == CardType.Adventure)
            {
                if (action.SourceCard.Owner.Adventures.Any())
                {
                    validator.Invalidate("Adventure already in play");
                }
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
            Global.Events.Unsubscribe(Notification.Prepare<SolveAdventureAction>(), OnPrepareSolveAdventure);
            Global.Events.Subscribe(Notification.Prepare<SolveAdventureAction>(), OnPrepareSolveAdventure);
        }
    }
}
