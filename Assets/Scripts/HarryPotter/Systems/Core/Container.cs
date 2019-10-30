using System.Collections.Generic;

namespace HarryPotter.Systems.Core
{
    public interface IContainer {
        T AddSystem<T> (string key = null) where T : ISystem, new ();
        T AddSystem<T> (T system, string key = null) where T : ISystem;
        T GetSystem<T> (string key = null) where T : ISystem;
        ICollection<ISystem> Systems();
    }
    
    public class Container : IContainer 
    {
        private readonly Dictionary<string, ISystem> _systems = new Dictionary<string, ISystem>();

        public T AddSystem<T> (string key = null) where T : ISystem, new() => AddSystem(new T(), key);
        
        public T AddSystem<T> (T system, string key = null) where T : ISystem 
        {
            key = key ?? typeof(T).Name;
            _systems.Add (key, system);
            system.Container = this;
            return system;
        }

        public T GetSystem<T> (string key = null) where T : ISystem {
            key = key ?? typeof(T).Name;
            var system = _systems.ContainsKey (key) ? (T)_systems [key] : default (T);
            return system;
        }

        public ICollection<ISystem> Systems() {
            return _systems.Values;
        }	
    }
}