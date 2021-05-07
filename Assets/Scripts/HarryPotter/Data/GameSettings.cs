using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public bool DebugMode;

        [Header("Overwrite Player Decks")]
        public CardData LocalStarting;
        public List<CardData> LocalDeck;
        
        [Space(5)]
        public CardData AIStarting; 
        public List<CardData> AIDeck;
    }
}