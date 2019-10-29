using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using UnityEngine;
using Utils;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Player")]
    public class Player : ScriptableObject
    {
        public HashSet<LessonType> LessonTypes
            => Cards.Where(c => c.ZoneType.IsInPlay())
                    .SelectMany(c => c.Data.Attributes)
                    .OfType<LessonProvider>()
                    .Select(p => p.Type)
                    .ToHashSet();

        public int LessonCount 
            => Cards.Where(c => c.ZoneType.IsInPlay())
                    .SelectMany(c => c.Data.Attributes)
                    .OfType<LessonProvider>()
                    .Sum(p => p.Amount);

        public int ActionsAvailable = 0;

        public List<CardData> StartingDeck = new List<CardData>();
        public CardData StartingCharacter;
        
        public List<Card> Cards = new List<Card>();

        private void Awake()
        {
            ResetState();
        }

        public void ResetState()
        {
            ActionsAvailable = 0;
            Cards.Clear();
            LessonTypes.Clear();
        }
    }
}