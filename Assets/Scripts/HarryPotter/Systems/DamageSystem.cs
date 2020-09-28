using HarryPotter.Enums;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Systems
{
    public class DamageSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<DamageAction>(), OnPerformDamage);
        }

        private void OnPerformDamage(object sender, object args)
        {
            var action = (DamageAction) args;

            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            action.Cards = action.Target[Zones.Deck].Draw(action.Amount);
            
            foreach (var card in action.Cards)
            {
                playerSystem.ChangeZone(card, Zones.Discard);
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<DamageAction>(), OnPerformDamage);            
        }
    }
}