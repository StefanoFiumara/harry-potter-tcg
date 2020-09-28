using System;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HarryPotter.UI
{
    public class PlayerHUDView : MonoBehaviour
    {
        private IContainer _gameContainer;
        private MatchData _match;
        private GameViewSystem _gameView;
        
        public Button EndTurnBtn;
        public Button DrawCardBtn;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _gameContainer = _gameView.Container;
            _match = _gameContainer.Match;
            
            Global.Events.Subscribe(Notification.Prepare<ChangeTurnAction>(), OnPrepareChangeTurn);
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;

            if (action.NextPlayerIndex == _match.LocalPlayer.Index)
            {
                EndTurnBtn.interactable = true;
                DrawCardBtn.interactable = true;
            }
        }

        private bool IsActive => 
            _match.CurrentPlayer.ControlMode == ControlMode.Local 
            && _gameView.IsIdle;

        private void OnPrepareChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;

            if (action.NextPlayerIndex == _match.EnemyPlayer.Index)
            {
                EndTurnBtn.interactable = false;
                DrawCardBtn.interactable = false;
                _gameView.Cursor.ResetCursor();
            }
        }

        public void OnClickChangeTurn()
        {
            if (IsActive)
            {
                _gameContainer.ChangeTurn();
            }
        }

        public void OnClickDrawCard()
        {
            if (IsActive)
            {
                var playerSystem = _gameContainer.GetSystem<PlayerSystem>();
                playerSystem.DrawCards(_match.CurrentPlayer, 1, true);
            }
        }

        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<ChangeTurnAction>(), OnPrepareChangeTurn);
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPrepareChangeTurn);
        }
    }
}