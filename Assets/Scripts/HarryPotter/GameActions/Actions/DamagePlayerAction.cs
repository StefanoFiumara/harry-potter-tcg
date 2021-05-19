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
    public class DamagePlayerAction : GameAction, IAbilityLoader
    {
        public Player Target { get; private set; }
        public int Amount { get; private set; }
        
        public List<Card> DiscardedCards { get; set; }

        public DamagePlayerAction(Card source, Player target, int amount)
        {
            SourceCard = source;
            Target = target;
            Amount = amount;
            Player = source.Owner;
        }

        public DamagePlayerAction()
        {
            
        }

        public void Load(IContainer game, Ability ability)
        {
            var parameter = DamageActionParameter.FromString(ability.GetParams(nameof(DamagePlayerAction)));

            Amount = parameter.DamageAmount;
            SourceCard = ability.Owner;
            Player = SourceCard.Owner;

            var enemyPlayer = game.GetMatch().Players.Single(p => Player.Index != p.Index);
            
            Target = parameter.WhichPlayer == Alliance.Ally 
                ? Player 
                : enemyPlayer;
        }

        public override string ToString()
        {
            return $"DamagePlayerAction - {SourceCard.Data.CardName} does {Amount} damage to {Target.PlayerName}";
        }
    }
}