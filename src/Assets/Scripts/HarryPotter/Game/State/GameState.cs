using UnityEngine;

namespace HarryPotter.Game.State
{
    [CreateAssetMenu(menuName = "HarryPotter/Game/Global Game State")]
    public class GameState : ScriptableObject
    {
        public PlayerState LocalPlayer;
        public PlayerState EnemyPlayer;
    }
}