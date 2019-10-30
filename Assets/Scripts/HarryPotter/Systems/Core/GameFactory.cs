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

            game.GetSystem<MatchSystem>().GameState = gameState;

            return game;
        }
    }
}