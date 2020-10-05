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

            foreach (var card in action.Cards)
            {
                var ability = card.GetAttribute<Ability>();
                if (ability != null && ability.Type == AbilityType.WhenPlayed)
                {
                    var reaction = new AbilityAction(ability);
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