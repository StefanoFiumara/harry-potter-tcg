using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class RestrictionSystem : GameSystem, IAwake, IDestroy
    {
        private MatchData _match;
        private TargetSystem _targetSystem;

        public void Awake()
        {
            _match = Container.GetMatch();
            _targetSystem = Container.GetSystem<TargetSystem>();
            Global.Events.Subscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;

            var cardsInPlay = _match.Players.SelectMany(p => p.CardsInPlay);

            foreach (var card in cardsInPlay)
            {
                var restrictions = card.GetAttributes<PlayRestriction>();
                foreach (var restriction in restrictions)
                {
                    if (IsRestricted(action.SourceCard, restriction))
                    {
                        validator.Invalidate($"Card is restricted by {restriction.Owner.Data.CardName}");
                    }
                }
            }
        }

        public bool IsRestricted(Card card, PlayRestriction restriction)
        {
            var players = _targetSystem.GetPlayers(restriction.Owner, restriction.WhichPlayer);

            return players.Contains(card.Owner) &&
                   restriction.RestrictedCardTypes.HasCardType(card.Data.Type);
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}
