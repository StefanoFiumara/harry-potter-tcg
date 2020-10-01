using System;
using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.PlayerActions
{
    public class DrawCardsAction : GameAction, IAbilityLoader
    {
        public bool UsePlayerAction { get; private set; }
        public int Amount { get; private set; }
        public List<Card> DrawnCards { get; set; }

        public DrawCardsAction(Player player, int amount, bool usePlayerAction = false)
        {
            UsePlayerAction = usePlayerAction;
            Player = player;
            Amount = amount;
        }

        public DrawCardsAction()
        {
            
        }

        public override string ToString()
        {
            return $"DrawCardsAction - Player {Player.Index} Draws {Amount}";
        }

        public void Load(IContainer game, Ability ability)
        {
            Amount = Convert.ToInt32(ability.UserInfo);
            UsePlayerAction = false; // TODO: Will ability loaded actions use player actions? don't think so.
        }
    }
}