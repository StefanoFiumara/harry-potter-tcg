using HarryPotter.Data;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class CreatureSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            
        }

        //TODO: Better name for this?
        public void PerformCreatureDamagePhase(Player player)
        {
            var damageSystem = Container.GetSystem<DamageSystem>();
            var enemyPlayer = Container.Match.OppositePlayer;
            
            foreach (var card in player.Creatures)
            {
                var creature = card.GetAttribute<Creature>();
                damageSystem.DamagePlayer(card, enemyPlayer, creature.Attack);
            }
        }

        public void Destroy()
        {
            
        }
    }
}