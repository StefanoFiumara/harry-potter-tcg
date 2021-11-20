using HarryPotter.Data;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;

namespace HarryPotter.Systems
{
    public class CreatureSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<CreatureDamagePhaseAction>(), OnPerformCreatureDamagePhase);
        }

        private void OnPerformCreatureDamagePhase(object sender, object args)
        {
            var action = (CreatureDamagePhaseAction)args;
            var damageSystem = Container.GetSystem<DamageSystem>();

            foreach (var card in action.Player.Creatures)
            {
                var creature = card.GetAttribute<Creature>();
                if (creature.Attack > 0)
                {
                    var enemyPlayer = card.Owner.EnemyPlayer;
                    damageSystem.DamagePlayer(card, enemyPlayer, creature.Attack);
                }
            }
        }

        public void ApplyCreatureDamage(Player player)
        {
            var action = new CreatureDamagePhaseAction(player);

            if (Container.GetSystem<ActionSystem>().IsActive)
            {
                Container.AddReaction(action);
            }
            else
            {
                Container.Perform(action);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<CreatureDamagePhaseAction>(), OnPerformCreatureDamagePhase);
        }
    }
}
