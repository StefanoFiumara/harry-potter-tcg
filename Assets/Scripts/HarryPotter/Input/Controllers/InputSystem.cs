using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards.TargetSelectors;
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
        
        public GameView GameView { get; set; }
        private Container InputStateContainer { get; set; }
        public StateMachine StateMachine { get; private set; }

        public CardView ActiveCard { get; private set; }
        public Player ActivePlayer { get; private set; }
        
        public List<ManualTargetSelector> EffectSelectors { get; set; }
        public List<ManualTargetSelector> ConditionSelectors { get; set; }
        
        public int ConditionsIndex { get; set; }
        public int EffectsIndex { get; set; }

        private void Awake()
        {
            GameView = GetComponent<GameView>(); 
            Game = GameView.Container;
            
            InputStateContainer = new Container();
            StateMachine = InputStateContainer.AddSystem<StateMachine>();
            
            InputStateContainer.AddSystem<WaitingForInputState>().InputController = this;
            
            InputStateContainer.AddSystem<PreviewState>().InputController = this;
            InputStateContainer.AddSystem<DiscardPilePreviewState>().InputController = this;
            
            InputStateContainer.AddSystem<PlayConditionTargetingState>().InputController = this;
            InputStateContainer.AddSystem<PlayEffectTargetingState>().InputController = this;
            
            InputStateContainer.AddSystem<ActivateConditionTargetingState>().InputController = this;
            InputStateContainer.AddSystem<ActivateEffectTargetingState>().InputController = this;
            
            InputStateContainer.AddSystem<ResetState>().InputController = this;
            
            

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

        public void SetActiveCard(CardView cardView)
        {
            ActiveCard = cardView;
            ActivePlayer = cardView.Card.Owner;
        }

        public void SetActivePlayer(Player player)
        {
            ActivePlayer = player;
        }

        public void ClearState()
        {
            ActiveCard = null;
            ActivePlayer = null;
            
            ConditionSelectors = null;
            EffectSelectors = null;
            ConditionsIndex = 0;
            EffectsIndex = 0;
        }

        private void OnDisable()
        {
            Global.Events.Unsubscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }
    }
}