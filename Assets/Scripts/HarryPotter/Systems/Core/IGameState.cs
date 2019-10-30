using HarryPotter.Data;

namespace HarryPotter.Systems.Core
{
    public interface IGameState
    {
        GameState GameState { get; set; }
    }
}