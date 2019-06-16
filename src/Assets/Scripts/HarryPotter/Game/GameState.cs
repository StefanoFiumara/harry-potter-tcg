using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Game
{
    [CreateAssetMenu(menuName = "HarryPotter/Game/Global Game State")]
    public class GameState : ScriptableObject
    {
        public PlayerState LocalPlayer;
        public PlayerState EnemyPlayer;
    }
}