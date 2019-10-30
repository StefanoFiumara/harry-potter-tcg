using System;
using HarryPotter.ActionSystem;
using HarryPotter.Data;
using UnityEngine;

namespace HarryPotter.Views
{
    public class GameView : MonoBehaviour
    {
        public GameState Game;

        private ActionStack _actionStack;

        public bool IsIdle => !_actionStack.IsActive;
        
        private void Awake()
        {
            if (Game == null)
            {
                throw new UnityException("GameView does not have GameData attached.");
            }
            
            _actionStack = new ActionStack(Game);
        }

        private void Start()
        {
            throw new NotImplementedException();
        }
    }
}