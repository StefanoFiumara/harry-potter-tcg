using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.ActionParameters;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class DrawCardsAction : GameAction, IAbilityLoader
    {
        public bool UsePlayerAction { get; private set; }
        public int Amount { get; private set; }
        public List<Card> DrawnCards { get; set; }

        public DrawCardsAction() { }
        
        public DrawCardsAction(Player player, int amount, bool usePlayerAction = false)
        {
            UsePlayerAction = usePlayerAction;
            Player = player;
            Amount = amount;
        }

        public void Load(IContainer game, Ability ability)
        {
            var parameter = DrawCardsActionParameter.FromString(ability.GetParams(nameof(DrawCardsAction)));
            
            Amount = parameter.Amount;
            UsePlayerAction = false;

            var allyPlayer = ability.Owner.Owner;
            var enemyPlayer = game.GetMatch().Players.Single(p => ability.Owner.Owner.Index != p.Index);
            
            Player = parameter.WhichPlayer == Alliance.Ally 
                ? allyPlayer
                : enemyPlayer;
        }

        public override string ToString()
        {
            return $"DrawCardsAction - {Player.PlayerName} Draws {Amount}.";
        }
    }
}