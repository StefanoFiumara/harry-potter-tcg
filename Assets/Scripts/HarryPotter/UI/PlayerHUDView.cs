using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter.UI
{
    public class PlayerHUDView : MonoBehaviour
    {
        private IContainer _gameContainer;
        private GameState _gameState;
        private GameViewSystem _gameView;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _gameContainer = _gameView.Container;
            _gameState = _gameContainer.GameState;
        }
        
        private bool IsActive => 
            _gameState.CurrentPlayer.ControlMode == ControlMode.Local 
            && _gameView.IsIdle;

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
                playerSystem.DrawCards(_gameState.CurrentPlayer, 1, true);
            }
        }

        public void OnClickBackToMainMenu()
        {
            SceneManager.LoadScene(Scenes.MAIN_MENU);
        }
    }
}