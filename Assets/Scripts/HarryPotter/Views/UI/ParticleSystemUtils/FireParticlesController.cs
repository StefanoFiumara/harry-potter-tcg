using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HarryPotter.Views.UI.ParticleSystemUtils
{
    public class FireParticlesController : MonoBehaviour
    {
        private List<ParticleSystem> _systems;

        private void Awake()
        {
            _systems = GetComponentsInChildren<ParticleSystem>().ToList();
            _systems.Add(GetComponent<ParticleSystem>());
        }

        public void Play()
        {
            foreach (var system in _systems)
            {
                system.Play();
            }
        }

        public void Stop()
        {
            foreach (var system in _systems)
            {
                system.Stop();
            }
        }
    }
}