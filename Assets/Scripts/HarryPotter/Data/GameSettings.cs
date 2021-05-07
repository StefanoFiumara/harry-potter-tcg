using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Data
{
    public class GameSettings : ScriptableObject
    {
        public bool DebugMode;

        public CardData Debug_LocalStarting;
        public List<CardData> Debug_LocalDeck;
        
        public CardData Debug_AIStarting; 
        public List<CardData> Debug_AIDeck;
    }
}