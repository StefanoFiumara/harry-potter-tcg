using HarryPotter.Data;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class MatchSystem : GameSystem
    {
        public MatchData Match { get; set; }
    }

    public static class MatchSystemExtensions
    {
        public static MatchData GetMatch(this IContainer container) => container.GetSystem<MatchSystem>().Match;
    }
}