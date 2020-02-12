using HarryPotter.Systems;
using HarryPotter.Systems.Core;

namespace HarryPotter.StateManagement
{
    public class GlobalGameState : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(ActionSystem.BEGIN_SEQUENCE_NOTIFICATION, OnBeginSequence);
            Global.Events.Subscribe(ActionSystem.COMPLETE_NOTIFICATION, OnCompleteAllActions);
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(ActionSystem.BEGIN_SEQUENCE_NOTIFICATION, OnBeginSequence);
            Global.Events.Unsubscribe(ActionSystem.COMPLETE_NOTIFICATION, OnCompleteAllActions);
        }

        private void OnBeginSequence(object sender, object args)
        {
            Container.ChangeState<SequenceState>();
        }

        private void OnCompleteAllActions(object sender, object args)
        {
            Container.ChangeState<PlayerIdleState>();
        }
    }
}