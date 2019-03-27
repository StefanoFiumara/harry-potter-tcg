using System;
using System.Collections.Generic;
using HarryPotter.Enums;
using HarryPotter.Game.Data;
using HarryPotter.UI;
using UnityEngine;

namespace HarryPotter.Game.State
{
    [CreateAssetMenu(menuName = "HarryPotter/Game/PlayerState")]
    public class PlayerState : ScriptableObject
    {
        public int ActionsAvailable = 0;

        public HashSet<LessonType> LessonTypes = new HashSet<LessonType>();
        public int LessonCount = 0;
        
        public List<CardData> StartingDeck = new List<CardData>();
        
        public void ResetState()
        {
            ActionsAvailable = 0;
            LessonCount = 0;
            LessonTypes.Clear();
        }
    }
}