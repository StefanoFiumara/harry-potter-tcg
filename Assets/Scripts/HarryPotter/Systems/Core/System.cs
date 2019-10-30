namespace HarryPotter.Systems.Core
{
    public interface ISystem
    {
        IContainer Container { get; set; }
    }

    public class System : ISystem
    {
        public IContainer Container {get; set; }
    }
}