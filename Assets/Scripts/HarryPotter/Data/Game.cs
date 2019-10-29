using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Game State")]
    public class Game : ScriptableObject
    {
        public Player LocalPlayer;
        public Player EnemyPlayer;
    }
}