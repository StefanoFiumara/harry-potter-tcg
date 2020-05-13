using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Systems
{
    public class TargetSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;

            var target = action.Card.GetAttribute<RequireTarget>();

            if (target == null || (!target.IsRequired && target.Selected == null))
            {
                return;
            }

            var candidates = GetMarks(target, target.Allowed);

            if (!candidates.Contains(target.Selected))
            {
                validator.Invalidate("Invalid target");
            }
        }

        //NOTE: For AI
        public void AutoTarget(Card card, ControlMode mode)
        {
            var target = card.GetAttribute<RequireTarget>();
            if (target == null)
            {
                return;
            }

            var mark = mode == ControlMode.Computer ? target.Preferred : target.Allowed;

            var candidates = GetMarks(target, mark);
            target.Selected = candidates.Count > 0 ? candidates.TakeRandom() : null;
        }

        public List<Card> GetMarks(RequireTarget source, Mark mark)
        {
            var marks = new List<Card>();
            var players = GetPlayers(source, mark);

            foreach (var player in players)
            {
                var cards = GetCards(source, mark, player);
                marks.AddRange(cards);
            }

            return marks;
        }

        List<Player> GetPlayers (RequireTarget source, Mark mark) {
            var card = source.Owner;
            var dataSystem = Container.GameState;
            var players = new List<Player> ();
            
            var pair = new Dictionary<Alliance, Player> () {
                { Alliance.Ally, dataSystem.Players[card.Owner.Index] }, 
                { Alliance.Enemy, dataSystem.Players[1 - card.Owner.Index] }
            };
            
            foreach (var key in pair.Keys) {
                if (mark.Alliance.Contains (key)) {
                    players.Add (pair[key]);
                }
            }
            return players;
        }

        List<Card> GetCards(RequireTarget source, Mark mark, Player player)
        {
            var cards = new List<Card>();

            var zones = new[]
            {
                Zones.Deck,
                Zones.Discard,
                Zones.Hand,
                Zones.Characters,
                Zones.Lessons,
                Zones.Creatures,
                Zones.Location,
            };

            foreach (Zones zone in zones)
            {
                if (mark.Zones.Contains(zone))
                {
                    cards.AddRange(player[zone]);
                }
            }

            return cards;
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}