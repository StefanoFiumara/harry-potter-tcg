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
            Global.Events.Subscribe(Notification.Validate<ActivateCardAction>(), OnValidateActivateCard);
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;
            
            ValidateAbilityPlayTargets(action, validator);
        }

        private void OnValidateActivateCard(object sender, object args)
        {
            var action = (ActivateCardAction) sender;
            var validator = (Validator) args;
            
            ValidateAbilityActivateTargets(action, validator);
        }

        private void ValidateAbilityActivateTargets(ActivateCardAction action, Validator validator)
        {
            var abilities = action.Card.GetAttributes<Ability>()
                .Where(a => a.Type == AbilityType.ActivateCondition || a.Type == AbilityType.ActivateEffect);

            foreach (var ability in abilities)
            {
                if (ability.TargetSelector != null)
                {
                    if (!ability.TargetSelector.HasEnoughTargets(Container, action.Card))
                    {
                        validator.Invalidate($"Not enough valid targets for {ability}");
                    }
                }
            }
        }
        
        private void ValidateAbilityPlayTargets(PlayCardAction action, Validator validator)
        {
            var abilities = action.Card.GetAttributes<Ability>()
                .Where(a => a.Type == AbilityType.PlayCondition || a.Type == AbilityType.PlayEffect);

            foreach (var ability in abilities)
            {
                if (ability.TargetSelector != null)
                {
                    if (!ability.TargetSelector.HasEnoughTargets(Container, action.Card))
                    {
                        validator.Invalidate($"Not enough valid targets for {ability}");
                    }
                }
            }
        }

        public void AutoTarget(Card card, AbilityType abilityType, ControlMode mode)
        {
            var abilities = card.GetAbilities(abilityType);
            foreach (var ability in abilities)
            {
                var targetSelector = ability.TargetSelector as ManualTargetSelector;
                if (targetSelector is null)
                {
                    continue;
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
        }

        public List<Card> GetTargetCandidates(Card source, Mark mark)
        {
            var marks = new List<Card>();
            var players = GetPlayers(source, mark.Alliance);

            foreach (var player in players)
            {
                var cards = GetCards(source, mark, player);
 
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

        private List<Card> GetCards(Card source, Mark mark, Player player)
        {
            var query = player.AllCards.Where(c => c.Zone.HasZone(mark.Zones) && c != source);
            
            if (mark.CardType != CardType.None)
            {
                query = query.Where(c => c.Data.Type.HasCardType(mark.CardType));
            }
            
            if (mark.LessonType != LessonType.None)
            {
                query = query.Where(c =>
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
            
            if (mark.RestrictedTags != Tag.None)
            {
                query = query.Where(card => !card.Data.Tags.HasTag(mark.RestrictedTags));
            }
                
            return query.ToList();
        }

        public List<Card> GetTargetCandidates(Card source, CardSearchQuery query, int maxAmount)
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

            if (query.LessonCostType != LessonType.None && query.LessonCostType != LessonType.Any)
            {
                cards = cards.Where(c =>
                {
                    var cost = c.GetAttribute<LessonCost>();
                    return cost != null && cost.Type.HasFlag(query.LessonCostType);
                });
            }

            if (query.LessonCostType != LessonType.None && query.LessonCostType != LessonType.Any)
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
                    return cost != null && cost.Amount <= query.MaxLessonCost;
                });
            }
            
            if (query.LessonProviderType != LessonType.None && query.LessonProviderType != LessonType.Any)
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

            if (query.Tags != Tag.None)
            {
                cards = cards.Where(c => c.Data.Tags.HasTag(query.Tags));
            }

            if (query.Zone != Zones.None)
            {
                cards = cards.Where(c => c.Zone.HasZone(query.Zone));
            }
            
            return cards.Take(maxAmount).ToList();
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}