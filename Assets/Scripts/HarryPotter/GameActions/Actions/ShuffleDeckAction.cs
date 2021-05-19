using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.GameActions.ActionParameters;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;

namespace HarryPotter.GameActions.Actions
{
    public class ShuffleDeckAction : GameAction, IAbilityLoader
    {
        public List<Player> Targets { get; set; }

        public ShuffleDeckAction() { }
        public ShuffleDeckAction(params Player[] players)
        {
            Targets = players.ToList();
        }
        
        public void Load(IContainer game, Ability ability)
        {
            var parameter = ShuffleDeckActionParameter.FromString(ability.GetParams(nameof(ShuffleDeckAction)));
            SourceCard = ability.Owner;
            
            var targetSystem = game.GetSystem<TargetSystem>();
            Targets = targetSystem.GetPlayers(ability.Owner, parameter.WhichPlayer);
        }

        public override string ToString()
        {
            return $"ShuffleDeckAction - {string.Join(", ", Targets.Select(t => t.PlayerName))} shuffle(s) their deck.";
        }
    }
}