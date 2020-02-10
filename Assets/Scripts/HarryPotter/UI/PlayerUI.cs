using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;

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
            _gameState = _gameContainer.GetSystem<MatchSystem>().GameState;
        }

        public void OnClickChangeTurn()
        {
            if (CanChangeTurn())
            {
                var matchSystem = _gameContainer.GetSystem<MatchSystem>();
                matchSystem.ChangeTurn();
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
        
        private bool CanChangeTurn()
        {
            return _gameState.CurrentPlayer.ControlMode == ControlMode.Local 
                   && _gameView.IsIdle;
        }

        private bool CanDrawCard()
        {
            return _gameState.CurrentPlayer.ControlMode == ControlMode.Local 
                   && _gameView.IsIdle 
                   && _gameState.CurrentPlayer.ActionsAvailable > 0;
        }
        
        //TODO: Animations for turn change and card draw effects goes here
    }
}