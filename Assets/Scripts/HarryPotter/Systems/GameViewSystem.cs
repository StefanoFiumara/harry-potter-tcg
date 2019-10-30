using HarryPotter.Data;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class GameViewSystem : MonoBehaviour, ISystem
    {
        public GameState Game;

        private ActionSystem _actionSystem;
        
        public bool IsIdle => !_actionSystem.IsActive;
        
        private void Awake()
        {
            if (Game == null)
            {
                throw new UnityException("GameView does not have GameData attached.");
            }
            
            _actionSystem = Container.GetSystem<ActionSystem>();
        }

        
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
    }
}