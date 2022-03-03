using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.GameActions;
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
        private IContainer _game;

        public GameView GameView { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public CardView ActiveCard { get; private set; }
        public Player ActivePlayer { get; private set; }
        public GameAction DesiredAction { get; private set; }

        public List<ManualTargetSelector> TargetSelectors { get; set; }
        public int SelectorIndex { get; set; }
        public int ConditionCount { get; set; }


        private void Awake()
        {
            GameView = GetComponent<GameView>();
            _game = GameView.Container;

            StateMachine = new StateMachine
            {
                Container = GameView.Container
            };

            StateMachine.AddState( new WaitingForInputState { InputController = this } );
            StateMachine.AddState( new PreviewState { InputController = this } );
            StateMachine.AddState( new DiscardPilePreviewState { InputController = this } );

            StateMachine.AddState( new TargetingState { InputController = this } );
            StateMachine.AddState( new CancelableTargetingState { InputController = this } );

            StateMachine.AddState( new ResetState { InputController = this } );

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

        public void SetDesiredAction(GameAction action)
        {
            DesiredAction = action;
        }

        public void PerformDesiredAction()
        {
            _game.Perform(DesiredAction);
            StateMachine.ChangeState<ResetState>();
        }

        public void ClearState()
        {
            ActiveCard = null;
            ActivePlayer = null;

            DesiredAction = null;

            TargetSelectors = null;
            SelectorIndex = 0;
            ConditionCount = 0;
        }

        private void OnDisable()
        {
            Global.Events.Unsubscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }
    }
}
