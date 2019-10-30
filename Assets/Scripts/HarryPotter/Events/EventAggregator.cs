using System;
using System.Collections.Generic;

namespace HarryPotter.Events
{
    public class EventAggregator
    {
        private Dictionary<string, IList<Action<object, object>>> EventCollection { get; }

        public EventAggregator()
        {
            EventCollection = new Dictionary<string, IList<Action<object, object>>>();
        }

        public void Subscribe(string eventName, Action<object, object> handler)
        {
            if (EventCollection.ContainsKey(eventName) == false)
            {
                EventCollection[eventName] = new List<Action<object, object>> { handler };
            }
            else
            {
                EventCollection[eventName].Add(handler);
            }
        }

        public void Unsubscribe(string eventName, Action<object, object> handler)
        {
            if (!EventCollection.ContainsKey(eventName)) return;

            while (EventCollection[eventName].Contains(handler))
            {
                EventCollection[eventName].Remove(handler);
            }
        }

        public void Publish(string eventName, object args = null, object sender = null)
        {
            if (!EventCollection.ContainsKey(eventName)) return;

            var actions = EventCollection[eventName];

            foreach (var action in actions)
            {
                action.Invoke(sender, args);
            }
        }

        public void Clear()
        {
            EventCollection.Clear();
        }
    }
}