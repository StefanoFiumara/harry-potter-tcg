namespace HarryPotter.Systems.Core
{
    public interface IGameSystem
    {
        IContainer Container { get; set; }
    }

    public class GameSystem : IGameSystem
    {
        public IContainer Container {get; set; }
    }
}