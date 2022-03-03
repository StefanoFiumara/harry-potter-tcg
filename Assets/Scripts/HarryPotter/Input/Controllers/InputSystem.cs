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
        public IContainer Game { get; private set; }

        public GameView GameView { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public CardView ActiveCard { get; private set; }
        public Player ActivePlayer { get; private set; }
        public GameAction DesiredAction { get; private set; }

        // TODO: Refactor into dynamic set of manual targeting steps depending on card clicked instead of hard coding effect -> condition -> reward
        public List<ManualTargetSelector> EffectSelectors { get; set; }
        public List<ManualTargetSelector> ConditionSelectors { get; set; }
        public List<ManualTargetSelector> RewardSelectors { get; set; }

        public int ConditionsIndex { get; set; }
        public int EffectsIndex { get; set; }
        public int RewardsIndex { get; set; }

        private void Awake()
        {
            GameView = GetComponent<GameView>();
            Game = GameView.Container;

            StateMachine = new StateMachine
            {
                Container = Game
            };

            StateMachine.AddState( new WaitingForInputState { InputController = this } );
            StateMachine.AddState( new PreviewState { InputController = this } );
            StateMachine.AddState( new DiscardPilePreviewState { InputController = this } );
            StateMachine.AddState( new ConditionTargetingState { InputController = this } );
            StateMachine.AddState( new EffectTargetingState { InputController = this } );
            StateMachine.AddState( new RewardsTargetingState { InputController = this } );
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
            Game.Perform(DesiredAction);
            StateMachine.ChangeState<ResetState>();
        }

        public void ClearState()
        {
            ActiveCard = null;
            ActivePlayer = null;

            ConditionSelectors = null;
            EffectSelectors = null;
            RewardSelectors = null;

            DesiredAction = null;

            ConditionsIndex = 0;
            EffectsIndex = 0;
            RewardsIndex = 0;
        }

        private void OnDisable()
        {
            Global.Events.Unsubscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }
    }
}
