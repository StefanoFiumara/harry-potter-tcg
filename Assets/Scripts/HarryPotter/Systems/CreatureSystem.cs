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

        //TODO: Convert to its own GameAction so that it can be triggered by cards like "Steelclaw"
        public void PerformCreatureDamagePhase(Player player)
        {
            var damageSystem = Container.GetSystem<DamageSystem>();

            foreach (var card in player.Creatures)
            {
                var creature = card.GetAttribute<Creature>();
                if (creature.Attack > 0)
                {
                    var enemyPlayer = card.Owner.EnemyPlayer;
                    damageSystem.DamagePlayer(card, enemyPlayer, creature.Attack);
                }
            }
        }

        public void Destroy()
        {

        }
    }
}
