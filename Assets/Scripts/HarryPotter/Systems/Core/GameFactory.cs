using HarryPotter.Data;
using HarryPotter.StateManagement;

namespace HarryPotter.Systems.Core
{
    public static class GameFactory
    {
        public static Container Create(GameState gameState)
        {
            var game = new Container(gameState);

            game.AddSystem<ActionSystem>();
            game.AddSystem<MatchSystem>();
            game.AddSystem<PlayerSystem>();

            // TODO: Only add AISystem when in Single Player Mode, otherwise, add NetworkSystem for multiplayer
            game.AddSystem<AISystem>();
            
            game.AddSystem<StateMachine>();
            game.AddSystem<GlobalGameState>();

            return game;
        }
    }
}