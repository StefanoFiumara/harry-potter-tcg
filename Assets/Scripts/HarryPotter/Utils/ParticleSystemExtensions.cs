using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Utils
{
    public static class ParticleSystemExtensions
    {
        public static void SetParticleColorGradient(this ParticleSystem system, LessonType particleColorType)
        {
            var colorKey = Colors.GetLessonColorGradient(particleColorType);
            var colorModule = system.colorOverLifetime;
            
            var gradient = new Gradient();
            var gradientColors = new []
            {
                new GradientColorKey(colorKey.Left, 0f),
                new GradientColorKey(colorKey.Right, 0.35f) 
            };
            var gradientAlphas = new []
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.5f),
                new GradientAlphaKey(0f, 1f),
            };
            
            gradient.SetKeys(gradientColors, gradientAlphas);
            colorModule.color = gradient;
        }
    }
}