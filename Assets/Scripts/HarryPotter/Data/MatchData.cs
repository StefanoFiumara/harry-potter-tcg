using System.Collections.Generic;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data
{
    public class MatchData : ScriptableObject
    {
        public const int LOCAL_PLAYER_INDEX = 0;
        public const int ENEMY_PLAYER_INDEX = 1;
        
        public List<Player> Players { get; private set; }
        
        public Player LocalPlayer;
        public Player EnemyPlayer;

        // TODO: CurrentPlayerIndex does not need to be serialized
        public int CurrentPlayerIndex;

        public Player CurrentPlayer => Players[CurrentPlayerIndex];
        public Player OppositePlayer => Players[1 - CurrentPlayerIndex];

        public void Initialize(GameSettings settings)
        {
            CurrentPlayerIndex = 0;
            Players = new List<Player>(2)
            {
                LocalPlayer, 
                EnemyPlayer
            };

            // TODO: Load player names from profile name?
            LocalPlayer.Index = LOCAL_PLAYER_INDEX;
            LocalPlayer.PlayerName = "Player 1";
            
            EnemyPlayer.Index = ENEMY_PLAYER_INDEX;
            EnemyPlayer.PlayerName = "Player 2";

            LocalPlayer.EnemyPlayer = EnemyPlayer;
            EnemyPlayer.EnemyPlayer = LocalPlayer;

            // TODO: set control mode based on scene - or maybe use GameSettings to set this up properly?
            Players[0].ControlMode = ControlMode.Local;
            Players[1].ControlMode = ControlMode.Computer;
            
            foreach (var player in Players)
            {
                player.Initialize(settings);
            }
        }
    }
}