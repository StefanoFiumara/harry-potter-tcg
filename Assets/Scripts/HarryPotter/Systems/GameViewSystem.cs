using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems.Core;
using HarryPotter.UI;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class GameViewSystem : MonoBehaviour, IGameSystem
    { 
        public GameState Game;
        public CardView CardPrefab;
     
        public TooltipController Tooltip { get; private set; }
        
        private ActionSystem _actionSystem;
     
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
        
        public bool IsIdle => !_actionSystem.IsActive && !Container.IsGameOver();
        

        private void Awake()
        {
            Tooltip = GetComponentInChildren<TooltipController>();
            
            if (Game == null)
            {
                Debug.LogError("GameView does not have GameData attached.");
                return;
            }

            if (Tooltip == null)
            {
                Debug.LogError("GameView could not find TooltipController.");
                return;
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
            
            var beginGame = new BeginGameAction();
            _actionSystem.Perform(beginGame);
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