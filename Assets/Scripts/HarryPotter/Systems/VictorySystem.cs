using System.Linq;
using HarryPotter.Data;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class VictorySystem : GameSystem
    {
        public const string GAME_OVER_NOTIFICATION = "VictorySystem.gameOverNotification";
        public bool IsGameOver()
        {
            return Container.GameState.Players.Any(p => p.Deck.Count == 0);
        }
    }

    public static class VictorySystemExtensions
    {
        public static bool IsGameOver(this IContainer game)
        {
            return game.GetSystem<VictorySystem>().IsGameOver();
        }
    }
}