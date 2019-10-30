using System.Linq;

namespace HarryPotter.Systems.Core
{
    public interface IAwake
    {
        void Awake();
    }

    public static class AwakeExtensions
    {
        public static void Awake(this IContainer container)
        {
            foreach (var system in container.Systems().OfType<IAwake>())
            {
                system.Awake();
            }
        }
    }
}