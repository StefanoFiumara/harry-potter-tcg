using System.Linq;

namespace HarryPotter.Systems.Core
{
    public interface IDestroy
    {
        void Destroy();
    }
    
    public static class DestroyExtensions
    {
        public static void Destroy(this IContainer container)
        {
            foreach (var system in container.Systems().OfType<IDestroy>())
            {
                system.Destroy();
            }
        }
    }
}