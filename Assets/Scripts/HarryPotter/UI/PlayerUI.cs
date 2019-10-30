using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.UI
{
    public class PlayerUI : MonoBehaviour
    {
        private IContainer _gameContainer; //TODO: Hook up game view system to this component
        
        private void Awake()
        {
            _gameContainer = GetComponentInParent<GameViewSystem>().Container;
        }

        public void OnClickChangeTurn()
        {
            Debug.Log("Change Turn Clicked");   
            if (CanChangeTurn())
            {
                Debug.Log("Changing Turn");
                var matchSystem = _gameContainer.GetSystem<MatchSystem>();
                matchSystem.ChangeTurn();
            }
            else
            {
                // TODO: Sound clip?
            }
        }
        
        private bool CanChangeTurn()
        {
            var gameState = _gameContainer.GetSystem<MatchSystem>().GameState;
            var gameView = _gameContainer.GetSystem<GameViewSystem>();
            return gameState.CurrentPlayer.ControlMode == ControlMode.Local && gameView.IsIdle;
        }
    }
}