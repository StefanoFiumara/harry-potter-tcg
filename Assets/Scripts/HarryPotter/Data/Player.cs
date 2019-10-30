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
        public ControlMode ControlMode;
        public int Id;
        
        public HashSet<LessonType> LessonTypes
            => AllCards.Where(c => c.Zone.IsInPlay())
                    .SelectMany(c => c.Data.Attributes)
                    .OfType<LessonProvider>()
                    .Select(p => p.Type)
                    .ToHashSet();

        public int LessonCount 
            => AllCards.Where(c => c.Zone.IsInPlay())
                    .SelectMany(c => c.Data.Attributes)
                    .OfType<LessonProvider>()
                    .Sum(p => p.Amount);

        public int ActionsAvailable = 0;

        public List<CardData> StartingDeck = new List<CardData>();
        public CardData StartingCharacter;

        public List<Card> AllCards   = new List<Card>();
        
        public List<Card> Deck       = new List<Card>();
        public List<Card> Discard    = new List<Card>();
        public List<Card> Hand       = new List<Card>();
        public List<Card> Characters = new List<Card>();
        public List<Card> Lessons    = new List<Card>();
        public List<Card> Creatures  = new List<Card>();
        public List<Card> Location   = new List<Card>();
        public List<Card> Match      = new List<Card>();
        public List<Card> Items      = new List<Card>();
        public List<Card> Adventure  = new List<Card>();

        
        public List<Card> this[Zones z] {
            get {
                switch (z) {
                    case Zones.Deck:       return Deck;
                    case Zones.Discard:    return Discard;
                    case Zones.Hand:       return Hand;
                    case Zones.Characters: return Characters;
                    case Zones.Lessons:    return Lessons;
                    case Zones.Creatures:  return Creatures;
                    case Zones.Location:   return Location;
                    case Zones.Match:      return Match;
                    case Zones.Items:      return Items;
                    case Zones.Adventure:  return Adventure;
                    default:
                        return null;
                }
            }
        }
        public void  Initialize()
        {
            ResetState();
            Debug.Log("Player Awake");
            foreach (var cardData in StartingDeck)
            {
                var card = new Card(cardData);
                AllCards.Add(card);
                Deck.Add(card);
            }
        }

        public void ResetState()
        {
            ActionsAvailable = 0;
            
            AllCards.Clear();
            Deck.Clear();
            Discard.Clear();   
            Hand.Clear();      
            Characters.Clear();
            Lessons.Clear();   
            Creatures.Clear(); 
            Location.Clear();  
            Match.Clear();     
            Items.Clear();     
            Adventure.Clear();
        }
    }
}