using System.Linq;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class UniquenessSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;

            if (!action.Card.Data.Tags.HasTag(Tag.Unique))
            {
                // No need to check uniqueness for this card.
                return;
            }
            
            var players = Container.GetMatch().Players;
            var startingCharacters = players.Select(p => p.Characters.First()).ToList();

            var cardsInPlay = players.SelectMany(p => p.CardsInPlay).Except(startingCharacters).ToList();
            
            foreach (var card in cardsInPlay)
            {
                if (string.IsNullOrWhiteSpace(card.Data.UniquenessKey) || string.IsNullOrWhiteSpace(action.Card.Data.UniquenessKey))
                {
                    if (card.Data.CardName == action.Card.Data.CardName)
                    {
                        validator.Invalidate($"Unique card is already in play (Compared by card name: {action.Card.Data.CardName}).");
                        break;
                    }
                }
                else
                {
                    if (card.Data.UniquenessKey == action.Card.Data.UniquenessKey)
                    {
                        validator.Invalidate($"Unique card is already in play (Compared by uniqueness key: {action.Card.Data.UniquenessKey}).");
                        break;
                    }
                }
            }
        }
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}