using System.Linq;
using HarryPotter.Data;

namespace HarryPotter.Systems.Core
{
    public static class GameFactory
    {
        public static Container Create(GameState gameState)
        {
            var game = new Container();

            game.AddSystem<ActionSystem>();
            game.AddSystem<MatchSystem>();
            game.AddSystem<PlayerSystem>();

            foreach (var system in game.Systems().OfType<IGameState>())
            {
                system.GameState = gameState;
            }

            return game;
        }
    }
}