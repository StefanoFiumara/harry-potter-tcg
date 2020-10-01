using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using Utils;

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
            
            var ability = action.Card.GetAttribute<Ability>();
            if (ability != null && ability.Type == AbilityType.WhenPlayed)
            {
                var reaction = new AbilityAction(ability);
                Container.AddReaction(reaction);
            }
        }

        private void OnPerformPlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;
            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            playerSystem.ChangeZone(action.Card, action.Card.Data.Type.ToTargetZone());
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Unsubscribe(Notification.Perform<PlayToBoardAction>(), OnPerformPlayToBoard);
        }
    }
}