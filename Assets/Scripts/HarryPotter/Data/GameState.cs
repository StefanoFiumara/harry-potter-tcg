using System.Collections.Generic;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Game State")]
    public class GameState : ScriptableObject
    {
        public const int LOCAL_PLAYER_INDEX = 0;
        public const int ENEMY_PLAYER_INDEX = 1;
        
        public List<Player> Players { get; private set; }
        
        public Player LocalPlayer;
        public Player EnemyPlayer;

        public int CurrentPlayerIndex;

        public Player CurrentPlayer => Players[CurrentPlayerIndex];
        public Player OppositePlayer => Players[1 - CurrentPlayerIndex];

        public void Initialize()
        {
            CurrentPlayerIndex = 0;
            Players = new List<Player>(2)
            {
                LocalPlayer, 
                EnemyPlayer
            };

            LocalPlayer.Index = LOCAL_PLAYER_INDEX;
            EnemyPlayer.Index = ENEMY_PLAYER_INDEX;

            foreach (var player in Players)
            {
                player.Initialize();
            }
        }
    }
}