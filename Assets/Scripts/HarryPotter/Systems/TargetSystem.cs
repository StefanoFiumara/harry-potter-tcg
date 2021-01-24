using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

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
            
            ValidateManualTarget(action, validator);
            ValidateAbilityTarget(action, validator);
        }

        private void ValidateAbilityTarget(PlayCardAction action, Validator validator)
        {
            var ability = action.Card.GetAttributes<Ability>().SingleOrDefault(a => a.Type == AbilityType.WhenPlayed);

            if (ability != null && ability.TargetSelector != null && !(ability.TargetSelector is ManualTargetSelector))
            {
                if (!ability.TargetSelector.HasEnoughTargets(Container, action.Card))
                {
                    validator.Invalidate($"Not enough valid targets for {ability}");
                }
            }
        }

        private void ValidateManualTarget(PlayCardAction action, Validator validator)
        {
            var targetSelector = action.Card.GetTargetSelector<ManualTargetSelector>(AbilityType.WhenPlayed);

            if (targetSelector == null)
            {
                return;
            }

            if (targetSelector.Selected.Count < targetSelector.RequiredAmount)
            {
                validator.Invalidate("Not enough valid targets for ManualTarget Attribute");
            }

            var candidates = GetTargetCandidates(action.Card, targetSelector.Allowed);

            foreach (var candidate in targetSelector.Selected)
            {
                if (!candidates.Contains(candidate))
                {
                    validator.Invalidate("Invalid target");
                }
            }
        }

        public void AutoTarget(Card card, AbilityType abilityType, ControlMode mode)
        {
            var targetSelector = card.GetTargetSelector<ManualTargetSelector>(abilityType);
            if (targetSelector is null)
            {
                return;
            }
            
            var candidates = GetTargetCandidates(card, targetSelector.Allowed);

            if (candidates.Count >= targetSelector.RequiredAmount)
            {
                int amountSelected = Mathf.Min(candidates.Count, targetSelector.MaxAmount);
                
                // IDEA: we could use Control Mode here to determine if we need a smarter system for target selection for the AI
                targetSelector.Selected = candidates.TakeRandom(amountSelected);
            }
            else
            {
                targetSelector.Selected.Clear();
            }
        }

        public List<Card> GetTargetCandidates(Card source, Mark mark)
        {
            var marks = new List<Card>();
            var players = GetPlayers(source, mark.Alliance);

            foreach (var player in players)
            {
                var cards = GetCards(mark, player);
                marks.AddRange(cards);
            }

            return marks;
        }

        public List<Player> GetPlayers (Card source, Alliance alliance) 
        {
            var allianceMap = new Dictionary<Alliance, Player> 
            {
                { Alliance.Ally , Container.Match.Players[source.Owner.Index]     }, 
                { Alliance.Enemy, Container.Match.Players[1 - source.Owner.Index] }
            };


            return allianceMap.Keys
                .Where(k => k.HasAlliance(alliance))
                .Select(k => allianceMap[k])
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

        public List<Card> GetTargetCandidates(Card source, CardSearchQuery query)
        {
            // TODO: Make this more compact?
            var cards = Container.Match.Players.SelectMany(p => p.AllCards);
             
            if (!string.IsNullOrWhiteSpace(query.CardName))
            {
                cards = cards.Where(c => c.Data.CardName.Contains(query.CardName));
            }

            if (query.CardType != CardType.None)
            {
                cards = cards.Where(c => c.Data.Type.HasCardType(query.CardType));
            }

            if (query.LessonCostType != LessonType.None || query.LessonCostType != LessonType.Any)
            {
                cards = cards.Where(c =>
                {
                    var cost = c.GetAttribute<LessonCost>();
                    return cost != null && cost.Type.HasFlag(query.LessonCostType);
                });
            }

            if (query.LessonCostType != LessonType.None || query.LessonCostType != LessonType.Any)
            {
                cards = cards.Where(c =>
                {
                    var cost = c.GetAttribute<LessonCost>();
                    return cost != null && cost.Type.HasFlag(query.LessonCostType);
                });
            }
            
            if (query.MinLessonCost != 0)
            {
                cards = cards.Where(c =>
                {
                    var cost = c.GetAttribute<LessonCost>();
                    return cost != null && cost.Amount >= query.MinLessonCost;
                });
            }
            
            if (query.MaxLessonCost != 0)
            {
                cards = cards.Where(c =>
                {
                    var cost = c.GetAttribute<LessonCost>();
                    return cost != null && cost.Amount <= query.MinLessonCost;
                });
            }
            
            if (query.LessonProviderType != LessonType.None || query.LessonProviderType != LessonType.Any)
            {
                cards = cards.Where(c =>
                {
                    var provider = c.GetAttribute<LessonProvider>();
                    return provider != null && provider.Type.HasFlag(query.LessonCostType);
                });
            }

            if (query.Ownership != Alliance.None)
            {
                var validOwners = GetPlayers(source, query.Ownership);
                cards = cards.Where(c => validOwners.Contains(c.Owner));
            }
            
            return cards.ToList();
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}