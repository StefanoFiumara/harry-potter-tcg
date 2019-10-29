using System.Collections.Generic;
using UnityEngine;

namespace HarryPotter.Events
{
    [CreateAssetMenu(menuName = "HarryPotter/Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if(!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        } 
    }
}
