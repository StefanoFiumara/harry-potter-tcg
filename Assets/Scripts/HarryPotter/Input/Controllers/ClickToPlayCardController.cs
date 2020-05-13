using HarryPotter.Input.InputStates;
using HarryPotter.StateManagement;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Input.Controllers
{
    public class ClickToPlayCardController : MonoBehaviour
    {
        public IContainer Game { get; set; }
        private Container Container { get; set; }
        public StateMachine StateMachine { get; private set; }

        public CardView ActiveCard { get; set; }

        //TODO: Is there a better way to detect active state so that we don't have to manually handle this?
        public bool IsCardPreview { get; set; }

        private void Awake()
        {
            Game = GetComponentInParent<GameViewSystem>().Container;
            
            Container = new Container(); //TODO: Code smell - Container with null GameState, do we add a GameContainer to the hierarchy?
            StateMachine = Container.AddSystem<StateMachine>();
            
            Container.AddSystem<WaitingForInputState>().Owner = this;
            Container.AddSystem<PreviewState>().Owner = this;
            Container.AddSystem<ConfirmOrCancelState>().Owner = this;
            Container.AddSystem<CancellingState>().Owner = this;
            Container.AddSystem<ConfirmState>().Owner = this;
            Container.AddSystem<ResetState>().Owner = this;
            Container.AddSystem<TargetingState>().Owner = this;

            IsCardPreview = false;
            StateMachine.ChangeState<WaitingForInputState>();
        }

        private void OnEnable()
        {
            Global.Events.Subscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }

        private void OnClickNotification(object sender, object args)
        {
            var handler = StateMachine.CurrentState as IClickableHandler;
            
            handler?.OnClickNotification(sender, args);
        }
        
        private void OnDisable()
        {
            Global.Events.Unsubscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }
    }
}