using System.Collections.Generic;
using HarryPotter.Enums;
using HarryPotter.Game.Data;
using UnityEngine;

namespace HarryPotter.Game.State
{
    [CreateAssetMenu(menuName = "HarryPotter/Game/PlayerState")]
    public class PlayerState : ScriptableObject
    {
        public HashSet<LessonType> LessonTypes = new HashSet<LessonType>();

        public int LessonCount = 0;
        public int ActionsAvailable = 0;

        public List<CardData> StartingDeck = new List<CardData>();
        public CardData StartingCharacter;
        
        public List<CardState> Cards = new List<CardState>();

        private void Awake()
        {
            ResetState();
        }

        public void ResetState()
        {
            ActionsAvailable = 0;
            LessonCount = 0;
            Cards.Clear();
            LessonTypes.Clear();
        }
    }
}