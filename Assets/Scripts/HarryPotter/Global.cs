using System;
using HarryPotter.EventSystem;
using UnityEngine;

namespace HarryPotter
{
    public static class Global
    {
        public static int GenerateId<T> () => GenerateId (typeof(T));
        public static int GenerateId(Type type) => Animator.StringToHash(type.Name);
    
        public static EventAggregator Events = new EventAggregator();
    }

    public static class Notification
    {
        public static string Prepare (Type t) => $"{t.Name}.PrepareNotification";
        public static string Perform (Type t) => $"{t.Name}.PerformNotification";
        
        public static string Prepare<T> () => Prepare (typeof(T));
        public static string Perform<T> () => Perform (typeof(T));
    }
}