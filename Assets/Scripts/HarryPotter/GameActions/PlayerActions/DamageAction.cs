using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.PlayerActions
{
    public class DamageAction : GameAction, IAbilityLoader
    {
        public Card Source { get; private set; }
        public Player Target { get; private set; }
        public int Amount { get; private set; }
        
        public List<Card> Cards { get; set; }

        public DamageAction(Card source, Player target, int amount)
        {
            Source = source;
            Target = target;
            Amount = amount;
            Player = source.Owner;
        }

        public DamageAction()
        {
            
        }

        public override string ToString()
        {
            return $"DamageAction - {Source.Data.CardName} does {Amount} damage to Player {Target.Index}";
        }

        public void Load(IContainer game, Ability ability)
        {
            Source = ability.Owner;
            Player = Source.Owner;
            Target = game.Match.Players.Single(p => Source.Owner.Index == p.Index);
            Amount = Convert.ToInt32(ability.UserInfo);
        }
    }
}