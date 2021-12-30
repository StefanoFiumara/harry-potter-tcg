using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class BoardSystem : GameSystem, IAwake, IDestroy
    {
        private AbilitySystem _abilitySystem;

        public void Awake()
        {
            _abilitySystem = Container.GetSystem<AbilitySystem>();

            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Subscribe(Notification.Prepare<ActivateCardAction>(), OnPrepareActivateCard);

            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Perform<PlayToBoardAction>(), OnPerformPlayToBoard);
        }

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.SourceCard.Data.Type != CardType.Spell)
            {
                var boardAction = new PlayToBoardAction(action.SourceCard);
                Container.AddReaction(boardAction);
            }
        }

        private void OnPrepareActivateCard(object sender, object args)
        {
            var action = (ActivateCardAction) args;

            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.ActivateCondition);
            _abilitySystem.TriggerAbility(action.SourceCard, AbilityType.ActivateEffect);
        }

        private void OnPreparePlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;

            // TODO: Do we need to set priority levels for these reactions?
            foreach (var card in action.Cards)
            {
                _abilitySystem.TriggerAbility(card, AbilityType.PlayCondition);
                _abilitySystem.TriggerAbility(card, AbilityType.PlayEffect);
            }
        }

        private void OnPerformPlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;
            var playerSystem = Container.GetSystem<PlayerSystem>();

            foreach (var card in action.Cards)
            {
                playerSystem.ChangeZone(card, card.Data.Type.ToTargetZone());
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Unsubscribe(Notification.Perform<ActivateCardAction>(), OnPrepareActivateCard);



            Global.Events.Unsubscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Perform<PlayToBoardAction>(), OnPerformPlayToBoard);
        }
    }
}
