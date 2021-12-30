using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Player")]
    public class Player : ScriptableObject
    {
        public ControlMode ControlMode { get; set; }

        public int Index { get; set; }

        public int ActionsAvailable { get; set; }

        // TODO: If we ever implement the loading of a PlayerProfile with the save manager, we might want to store the selected deck and player name there, instead of here.
        // IDEA: Or PlayerProfile could be a property of this class that gets loaded in when the save manager loads.
        public Deck SelectedDeck { get; set; }

        public string PlayerName { get; set; }

        public Player EnemyPlayer { get; set; }

        public List<Card> AllCards  { get; }

        public List<Card> Deck       { get; }
        public List<Card> Discard    { get; }
        public List<Card> Hand       { get; }
        public List<Card> Characters { get; }
        public List<Card> Lessons    { get; }
        public List<Card> Creatures  { get; }
        public List<Card> Location   { get; }
        public List<Card> Match      { get; }
        public List<Card> Items      { get; }
        public List<Card> Adventures  { get; }

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
                    case Zones.Adventure:  return Adventures;
                    default:
                        return null;
                }
            }
        }

        public Player()
        {
            AllCards   = new List<Card>();
            Deck       = new List<Card>();
            Discard    = new List<Card>();
            Hand       = new List<Card>();
            Characters = new List<Card>();
            Lessons    = new List<Card>();
            Creatures  = new List<Card>();
            Location   = new List<Card>();
            Match      = new List<Card>();
            Items      = new List<Card>();
            Adventures  = new List<Card>();
        }

        public void Initialize(GameSettings settings)
        {
            ResetState();

            var (deck, startingCharacter) = LoadPlayerData(settings);

            foreach (var cardData in deck.Where(c => c != null))
            {
                var card = new Card(cardData, this);

                AllCards.Add(card);
                Deck.Add(card);
            }

            if (startingCharacter != null)
            {
                var card = new Card(startingCharacter, this, Zones.Characters);

                Characters.Add(card);
                AllCards.Add(card);
            }
        }

        public (List<CardData> deck, CardData startingCharacter) LoadPlayerData(GameSettings settings)
        {
            if (!settings.DebugMode)
            {
                return (SelectedDeck.Cards, SelectedDeck.StartingCharacter);
            }

            List<CardData> deck;
            CardData startingChar;

            if (settings.OverridePlayerDeck && ControlMode == ControlMode.Local)
            {
                deck = settings.LocalDeck;
                startingChar = settings.LocalStarting;

            }
            else if(settings.OverrideAIDeck && ControlMode == ControlMode.Computer)
            {
                deck = settings.AIDeck;
                startingChar = settings.AIStarting;
            }
            else
            {
                return (SelectedDeck.Cards, SelectedDeck.StartingCharacter);
            }

            return (deck, startingChar);

        }

        public HashSet<LessonType> LessonTypes => AllCards.Where(c => c.Zone.IsInPlay()).GetLessonProviderTypes();

        public int LessonCount
            => AllCards.Where(c => c.Zone.IsInPlay())
                .SelectMany(c => c.Attributes)
                .OfType<LessonProvider>()
                .Sum(p => p.Amount);

        public List<Card> CardsInPlay => AllCards.Where(c => c.Zone.IsInPlay()).ToList();

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
            Adventures.Clear();
        }
    }
}
