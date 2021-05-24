using System.Collections.Generic;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Views.UI.ParticleSystemUtils
{
    public class ParticleSystemController : MonoBehaviour
    {
        public FireParticlesController Creatures;
        public FireParticlesController Charms;
        public FireParticlesController Transfiguration;
        public FireParticlesController Potions;
        public FireParticlesController Quidditch;
        public FireParticlesController Neutral;

        private Dictionary<LessonType, FireParticlesController> _particleMap;

        private FireParticlesController _activeParticles;
        
        private void Start()
        {
            _particleMap = new Dictionary<LessonType, FireParticlesController>
            {
                { LessonType.Creatures,       Creatures},
                { LessonType.Charms,          Charms},
                { LessonType.Transfiguration, Transfiguration},
                { LessonType.Potions,         Potions},
                { LessonType.Quidditch,       Quidditch},
                { LessonType.None,            Neutral}
            };
            
            Stop();
        }

        public void SetParticleColor(LessonType lessonType)
        {
            _activeParticles = _particleMap[lessonType];
        }

        public void Play()
        {
            _activeParticles.Play();
        }

        public void Stop()
        {
            foreach (var controller in _particleMap)
            {
                controller.Value.Stop();
            }   
        }
    }
}