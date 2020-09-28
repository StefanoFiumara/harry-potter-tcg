using System.Collections.Generic;
using System.Linq;
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

            if (target == null)
            {
                return;
            }

            if (target.Selected.Count < target.RequiredAmount)
            {
                validator.Invalidate("Not enough valid targets");
            }
                
            var candidates = GetMarks(target, target.Allowed);

            foreach (var candidate in target.Selected)
            {
                if (!candidates.Contains(candidate))
                {
                    validator.Invalidate("Invalid target");
                }
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


            if (candidates.Count >= target.RequiredAmount)
            {
                target.Selected = candidates.TakeRandom(target.RequiredAmount);
            }
            else
            {
                target.Selected.Clear();
            }
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
            
            var players = new List<Player> ();
            
            var pair = new Dictionary<Alliance, Player> () {
                { Alliance.Ally, Container.Match.Players[card.Owner.Index] }, 
                { Alliance.Enemy, Container.Match.Players[1 - card.Owner.Index] }
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

            foreach (var zone in zones)
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