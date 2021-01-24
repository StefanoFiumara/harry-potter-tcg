using HarryPotter.Input.InputStates;
using HarryPotter.StateManagement;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Input.Controllers
{
    public class InputSystem : MonoBehaviour
    {
        public IContainer Game { get; set; }
        
        public GameViewSystem GameView { get; set; }
        private Container InputStateContainer { get; set; }
        public StateMachine StateMachine { get; private set; }

        public CardView ActiveCard { get; set; }

        private void Awake()
        {
            GameView = GetComponent<GameViewSystem>(); 
            Game = GameView.Container;
            
            InputStateContainer = new Container(); //TODO: Code smell - Container with null Match, add match system to hold match data for the game container instead.
            StateMachine = InputStateContainer.AddSystem<StateMachine>();
            
            InputStateContainer.AddSystem<WaitingForInputState>().InputSystem = this;
            InputStateContainer.AddSystem<PreviewState>().InputSystem = this;
            InputStateContainer.AddSystem<ConfirmOrCancelState>().InputSystem = this;
            InputStateContainer.AddSystem<CancellingState>().InputSystem = this;
            InputStateContainer.AddSystem<ConfirmState>().InputSystem = this;
            InputStateContainer.AddSystem<ResetState>().InputSystem = this;
            InputStateContainer.AddSystem<PlayEffectTargetingState>().InputSystem = this;
            InputStateContainer.AddSystem<PlayConditionTargetingState>().InputSystem = this;

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