using System.Linq;
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

            if (!action.SourceCard.Data.Tags.HasTag(Tag.Unique))
            {
                // No need to check uniqueness for this card.
                return;
            }
            
            var players = Container.GetMatch().Players;
            
            var cardsInPlay = players.SelectMany(p => p.CardsInPlay).ToList();
            
            foreach (var card in cardsInPlay)
            {
                if (string.IsNullOrWhiteSpace(card.Data.UniquenessKey) || string.IsNullOrWhiteSpace(action.SourceCard.Data.UniquenessKey))
                {
                    if (card.Data.CardName == action.SourceCard.Data.CardName)
                    {
                        validator.Invalidate($"Unique card is already in play (Compared by card name: {action.SourceCard.Data.CardName}).");
                        break;
                    }
                }
                else
                {
                    if (card.Data.UniquenessKey == action.SourceCard.Data.UniquenessKey)
                    {
                        validator.Invalidate($"Unique card is already in play (Compared by uniqueness key: {action.SourceCard.Data.UniquenessKey}).");
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