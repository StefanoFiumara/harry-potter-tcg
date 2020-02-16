using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter.UI
{
    public class PlayerUI : MonoBehaviour
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

        public void OnClickChangeTurn()
        {
            if (CanChangeTurn())
            {
                _gameContainer.ChangeTurn();
            }
            else
            {
                Debug.Log("Cannot Change Turn");
                // TODO: Sound clip?
            }
        }

        public void OnClickDrawCard()
        {
            if (CanDrawCard())
            {
                var playerSystem = _gameContainer.GetSystem<PlayerSystem>();
                playerSystem.DrawCards(_gameState.CurrentPlayer, 1, true);
            }
            else
            {
                Debug.Log("Cannot Draw Card.");
                // TODO: Sound clip?
                // TODO: Reason draw failed?
            }
        }

        public void OnClickBackToMainMenu()
        {
            SceneManager.LoadScene(Scenes.MAIN_MENU);
        }
        
        private bool CanChangeTurn()
        {
            return _gameState.CurrentPlayer.ControlMode == ControlMode.Local 
                   && _gameView.IsIdle;
        }

        private bool CanDrawCard()
        {
            return _gameState.CurrentPlayer.ControlMode == ControlMode.Local 
                   && _gameView.IsIdle 
                   // TODO: change this to use new validation system for Game Actions (i.e. let the game actions propagate and be canceled by the validation system).
                   && _gameState.CurrentPlayer.ActionsAvailable > 0; 
        }
    }
}