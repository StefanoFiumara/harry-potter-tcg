using System.Collections.Generic;
using HarryPotter.Data.Cards;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public float TweenTimescale;
        
        public bool DebugMode;
        
        [HideInInspector] public bool OverridePlayerDeck;
        [HideInInspector] public CardData LocalStarting;
        [HideInInspector] public List<CardData> LocalDeck;

        [HideInInspector] public bool OverrideAIDeck;
        [HideInInspector] public CardData AIStarting; 
        [HideInInspector] public List<CardData> AIDeck;
    }
}