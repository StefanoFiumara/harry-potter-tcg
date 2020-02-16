using System.Collections.Generic;
using HarryPotter.Data;

namespace HarryPotter.Systems.Core
{
    public interface IContainer {
        GameState GameState { get; }
        
        T AddSystem<T> (string key = null) where T : IGameSystem, new ();
        T AddSystem<T> (T system, string key = null) where T : IGameSystem;
        T GetSystem<T> (string key = null) where T : IGameSystem;
        ICollection<IGameSystem> Systems();
    }
    
    public class Container : IContainer 
    {
        private readonly Dictionary<string, IGameSystem> _systems = new Dictionary<string, IGameSystem>();
        public GameState GameState { get; }

        public Container(GameState gameState)
        {
            GameState = gameState;
            
            GameState.Initialize();
        }

        public Container()
        {
            
        }
        
        public T AddSystem<T> (string key = null) where T : IGameSystem, new() => AddSystem(new T(), key);
        
        public T AddSystem<T> (T system, string key = null) where T : IGameSystem 
        {
            key = key ?? typeof(T).Name;
            _systems.Add (key, system);
            system.Container = this;
            return system;
        }

        public T GetSystem<T> (string key = null) where T : IGameSystem {
            key = key ?? typeof(T).Name;
            var system = _systems.ContainsKey (key) ? (T)_systems [key] : default (T);
            return system;
        }

        public ICollection<IGameSystem> Systems() {
            return _systems.Values;
        }	
    }
}