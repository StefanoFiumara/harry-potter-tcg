using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class PlayerSystem : GameSystem, IAwake, IDestroy
    {
        private const int STARTING_HAND_AMOUNT = 7;
        
        private HandSystem _handSystem;
        private CreatureSystem _creatureSystem;

        public void Awake()
        {
            _handSystem = Container.GetSystem<HandSystem>();
            _creatureSystem = Container.GetSystem<CreatureSystem>();
            
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Subscribe(Notification.Prepare<BeginGameAction>(), OnPrepareGameBegin);
        }

        private void OnPrepareGameBegin(object sender, object args)
        {
            _handSystem.DrawCards(Container.Match.LocalPlayer, STARTING_HAND_AMOUNT);
            _handSystem.DrawCards(Container.Match.EnemyPlayer, STARTING_HAND_AMOUNT);
        }
        
        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            var player = Container.Match.Players[action.NextPlayerIndex];
            _handSystem.DrawCards(player, 1);
            _creatureSystem.DoCreatureDamagePhase(player);
        }

        public void ChangeZone(Card card, Zones zone, Player toPlayer = null)
        {
            var fromPlayer = card.Owner;
            toPlayer = toPlayer != null ? toPlayer : fromPlayer;

            if (card.Zone != Zones.None)
            {
                fromPlayer[card.Zone].Remove(card);    
            }

            if (zone != Zones.None)
            {
                toPlayer[zone]?.Add(card);
            }
            
            card.Zone = zone;
            card.Owner = toPlayer;
        }
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Unsubscribe(Notification.Prepare<BeginGameAction>(), OnPrepareGameBegin);
        }
    }
}