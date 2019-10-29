using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Game
{
    [CreateAssetMenu(menuName = "HarryPotter/Game/Global Game State")]
    public class GameState : ScriptableObject
    {
        public Player.Player LocalPlayer;
        public Player.Player EnemyPlayer;
    }
}