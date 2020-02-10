using System;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class GameViewSystem : MonoBehaviour, IGameSystem
    {
        private IContainer _container;
        public IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = GameFactory.Create(Game);
                    _container.AddSystem(this);
                }

                return _container;
            }
            
            set => _container = value;
        }
        
        public GameState Game;

        private ActionSystem _actionSystem;
        
        public bool IsIdle => !_actionSystem.IsActive;
        
        private void Awake()
        {
            if (Game == null)
            {
                throw new UnityException("GameView does not have GameData attached.");
            }
            
            Container.Awake();
            _actionSystem = Container.GetSystem<ActionSystem>();
        }

        private void Start()
        {
            SetupSinglePlayer();
        }
        
        private void SetupSinglePlayer() 
        {
            Game.Players[0].ControlMode = ControlMode.Local;
            Game.Players[1].ControlMode = ControlMode.Computer;
            
            var action = new ChangeTurnAction(Game.CurrentPlayerIndex);
            _actionSystem.Perform(action);
        }
        
        private void Update()
        {
            _actionSystem.Update();
        }

        private void OnDestroy()
        {
            Container.Destroy();
        }
    }
}