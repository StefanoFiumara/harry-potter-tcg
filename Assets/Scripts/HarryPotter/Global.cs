using System;
using HarryPotter.EventSystem;
using UnityEngine;

namespace HarryPotter
{
    public static class Global
    {
        public static int GenerateId<T> () => GenerateId (typeof(T));
        public static int GenerateId(Type type) => Animator.StringToHash(type.Name);
    
    
        public static string PrepareNotification (Type t) => $"{t.Name}.PrepareNotification";
        public static string PerformNotification (Type t) => $"{t.Name}.PerformNotification";
        
        public static string PrepareNotification<T> () => PrepareNotification (typeof(T));
        public static string PerformNotification<T> () => PerformNotification (typeof(T));
        
        public static EventAggregator Events = new EventAggregator();
    }
}