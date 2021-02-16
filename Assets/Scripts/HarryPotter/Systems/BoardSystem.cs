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
            Global.Events.Subscribe(Notification.Prepare<ActivateCardAction>(), OnPrepareActivateCard);
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

        private void OnPrepareActivateCard(object sender, object args)
        {
            var action = (ActivateCardAction) args;
            
            var effectAbilities = action.Card.GetAbilities(AbilityType.ActivateEffect);
            var conditionAbilities = action.Card.GetAbilities(AbilityType.ActivateCondition);

            foreach (var ability in conditionAbilities)
            {
                var reaction = new AbilityAction(ability);
                Container.AddReaction(reaction);
            }
            
            foreach (var ability in effectAbilities)
            {
                var reaction = new AbilityAction(ability);
                Container.AddReaction(reaction);
            }
        }
        
        private void OnPreparePlayToBoard(object sender, object args)
        {
            var action = (PlayToBoardAction) args;

            // TODO: Do we need to set priority levels for these reactions?
            foreach (var card in action.Cards)
            {
                var playEffectAbilities = card.GetAbilities(AbilityType.PlayEffect);
                var conditionAbilities = card.GetAbilities(AbilityType.PlayCondition);
                
                foreach (var ability in conditionAbilities)
                {
                    var reaction = new AbilityAction(ability);
                    Container.AddReaction(reaction);
                }
                
                foreach (var ability in playEffectAbilities)
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
            Global.Events.Unsubscribe(Notification.Perform<ActivateCardAction>(), OnPrepareActivateCard);
            
            Global.Events.Unsubscribe(Notification.Prepare<PlayToBoardAction>(), OnPreparePlayToBoard);
            Global.Events.Unsubscribe(Notification.Perform<PlayToBoardAction>(), OnPerformPlayToBoard);
        }
    }
}