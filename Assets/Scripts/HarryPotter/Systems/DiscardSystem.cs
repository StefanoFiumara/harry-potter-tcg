using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class DiscardSystem : GameSystem, IAwake, IDestroy
    {
        private PlayerSystem _playerSystem;

        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<DiscardAction>(), OnPerformDiscard);
            _playerSystem = Container.GetSystem<PlayerSystem>();
        }


        public void DiscardCard(Card source, Card target, GameAction sourceAction = null)
        {
            var action = new DiscardAction
            {
                SourceCard = source,
                Player = source.Owner,
                SourceAction = sourceAction,
                DiscardedCards = new List<Card> {target}
            };

            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
            }
        }
        
        private void OnPerformDiscard(object sender, object args)
        {
            var action = (DiscardAction) args;
            
            for (var i = action.DiscardedCards.Count - 1; i >= 0; i--)
            {
                var card = action.DiscardedCards[i];
                _playerSystem.ChangeZone(card, Zones.Discard);

                // TODO: Figure out when to reset attributes - Resetting on this action could clear out target selectors for subsequent reactions
                //       Alternatively, adjust ResetAttribute so that it does not reset TargetSelector values, since those are always reset by the InputSystem when necessary.
                // foreach (var attribute in card.Attributes)
                // {
                //     attribute.ResetAttribute();
                // }
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<DiscardAction>(), OnPerformDiscard);            
        }
    }
}