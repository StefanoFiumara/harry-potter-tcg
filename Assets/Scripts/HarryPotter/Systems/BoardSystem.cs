using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class BoardSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Subscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Subscribe(Notification.Perform<PlayToBoardAction>(), OnPerformPlayToBoard);
        }

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.Card.Data.Type != CardType.Spell)
            {
                var boardAction = new PlayToBoardAction(action.Card);
                Container.AddReaction(boardAction);
            }
        }

        private void OnPreparePlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;

            // TODO: Do we need to set priority levels for these reactions?
            foreach (var card in action.Cards)
            {
                var conditionAbility = card.GetAbility(AbilityType.PlayCondition);

                if (conditionAbility != null)
                {
                    var reaction = new AbilityAction(conditionAbility);
                    Container.AddReaction(reaction);
                }
                
                var whenPlayedAbility = card.GetAbility(AbilityType.PlayEffect);
                if (whenPlayedAbility != null)
                {
                    var reaction = new AbilityAction(whenPlayedAbility);
                    Container.AddReaction(reaction);
                }
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
            Global.Events.Unsubscribe(Notification.Perform<PlayToBoardAction>(), OnPerformPlayToBoard);
        }
    }
}