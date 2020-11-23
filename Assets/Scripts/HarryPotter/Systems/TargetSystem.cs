using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

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

            //TODO: Need to check for abilities that have other types of target selectors.
            ValidateManualTarget(action, validator);
            ValidateAbilityTarget(action, validator);
        }

        private void ValidateAbilityTarget(PlayCardAction action, Validator validator)
        {
            var ability = action.Card.GetAttributes<Ability>().SingleOrDefault(a => a.Type == AbilityType.WhenPlayed);

            if (ability != null && ability.TargetSelector != null)
            {
                if (!ability.TargetSelector.HasEnoughTargets(Container, action.Card))
                {
                    validator.Invalidate("Not enough valid targets");
                }
            }
        }

        private void ValidateManualTarget(PlayCardAction action, Validator validator)
        {
            var target = action.Card.GetAttribute<ManualTarget>();

            if (target == null)
            {
                return;
            }

            if (target.Selected.Count < target.RequiredAmount)
            {
                validator.Invalidate("Not enough valid targets");
            }

            var candidates = GetTargetCandidates(action.Card, target.Allowed);

            foreach (var candidate in target.Selected)
            {
                if (!candidates.Contains(candidate))
                {
                    validator.Invalidate("Invalid target");
                }
            }
        }

        public void AutoTarget(Card card, ControlMode mode)
        {
            var target = card.GetAttribute<ManualTarget>();
            if (target == null)
            {
                return;
            }

            // TODO: Control Mode here would potentially drive some kind of priority system for target selection, rather than randomly from the list of available candidates. 
            var candidates = GetTargetCandidates(card, target.Allowed);

            if (candidates.Count >= target.RequiredAmount)
            {
                target.Selected = candidates.TakeRandom(target.RequiredAmount);
            }
            else
            {
                target.Selected.Clear();
            }
        }

        public List<Card> GetTargetCandidates(Card source, Mark mark)
        {
            var marks = new List<Card>();
            var players = GetPlayers(source, mark);

            foreach (var player in players)
            {
                var cards = GetCards(mark, player);
                marks.AddRange(cards);
            }

            return marks;
        }

        List<Player> GetPlayers (Card source, Mark mark) 
        {
            var allianceMap = new Dictionary<Alliance, Player> 
            {
                { Alliance.Ally , Container.Match.Players[source.Owner.Index]     }, 
                { Alliance.Enemy, Container.Match.Players[1 - source.Owner.Index] }
            };


            return allianceMap.Keys
                .Where(alliance => alliance.HasAlliance(mark.Alliance))
                .Select(key => allianceMap[key])
                .ToList();
        }

        private List<Card> GetCards(Mark mark, Player player)
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
                if (zone.HasZone(mark.Zones))
                {
                    var eligibleCards = player[zone].AsEnumerable();
                    
                    if (mark.CardType != CardType.None)
                    {
                        eligibleCards = eligibleCards.Where(c => c.Data.Type.HasCardType(mark.CardType));
                    }
                    
                    if (mark.LessonType != LessonType.Any)
                    {
                        eligibleCards = eligibleCards.Where(c =>
                        {
                            // TODO: Could be ambiguous if targeting characters that provide lessons ?
                            var provider = c.GetAttribute<LessonProvider>();
                            if (provider != null)
                            {
                                return provider.Type.HasLessonType(mark.LessonType);
                            }
                            
                            var cost = c.GetAttribute<LessonCost>();
                            
                            return cost != null && cost.Type.HasLessonType(mark.LessonType);
                        });
                    }
                    
                    cards.AddRange(eligibleCards);
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