using System.Linq;
using HarryPotter.Data;

namespace HarryPotter.Systems.Core
{
    public static class GameFactory
    {
        public static Container Create(GameState gameState)
        {
            var game = new Container(gameState);

            game.GameState.Initialize();
            
            game.AddSystem<ActionSystem>();
            game.AddSystem<MatchSystem>();
            game.AddSystem<PlayerSystem>();

            return game;
        }
    }
}