using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Save
{
    public class SaveManager : MonoBehaviour
    {
        // TODO: Singleton?
        private string _profilePath;

        public CardLibrary Library;
        public Player LocalPlayer;

        private void Awake()
        {
            // TODO: don't hardcode this filename, maybe?
            _profilePath = Path.Combine(Application.persistentDataPath, "hptcg_profile.json");
            
            if (!File.Exists(_profilePath))
            {
                Debug.Log($"No player profile found - Creating new player profile in {_profilePath}");
                var newDeckId = Guid.NewGuid().ToString();
                
                var newPlayerProfile = new LocalPlayerProfile
                {
                    ProfileName = "New Player",
                    SelectedDeckId = newDeckId,
                    Decks = new List<Deck>
                    {
                        // TEMP: Create a default deck slot for the player, this can probably go away when we implement multiple deck management
                        new Deck
                        {
                            Id = newDeckId,  
                            Name = "New Deck", 
                            Cards = new List<string> { Library.Cards.First().Id },
                            StartingCharacterId = Library.Cards.First(c => c.CardName == "Hermione Granger").Id,
                        }
                    }
                };

                var serialized = JsonUtility.ToJson(newPlayerProfile, prettyPrint: true);
                File.AppendAllText(_profilePath, serialized);
            }

            LoadData();
        }
        
        public void LoadData()
        {
            var profileData = File.ReadAllText(_profilePath);
            var playerProfile = JsonUtility.FromJson<LocalPlayerProfile>(profileData);

            // TODO: Is deckToLoad ever going to be null?
            var deckToLoad = playerProfile.Decks.FirstOrDefault(d => d.Id == playerProfile.SelectedDeckId) ?? playerProfile.Decks.First();

            // TODO: Error handling when GetById fails ... Do we need to denote the game version in each save file to ensure these are valid?
            LocalPlayer.PlayerName = playerProfile.ProfileName;
            LocalPlayer.DeckId = deckToLoad.Id;
            LocalPlayer.DeckName = deckToLoad.Name;
            LocalPlayer.StartingCharacter = Library.GetById(deckToLoad.StartingCharacterId);

            LocalPlayer.StartingDeck = new List<CardData>();
            
            foreach (var cardId in deckToLoad.Cards)
            {
                var cardData = Library.GetById(cardId);
                LocalPlayer.StartingDeck.Add(cardData);
            }
        }

        public void SaveData()
        {
            var deckId = LocalPlayer.DeckId;
            var deckName = LocalPlayer.DeckName;
            var startingCharacter = LocalPlayer.StartingCharacter.Id;

            var cards = LocalPlayer.StartingDeck.Select(c => c.Id).ToList();

            var updatedDeck = new Deck
            {
                Id = deckId,
                Name = deckName,
                StartingCharacterId = startingCharacter,
                Cards = cards
            };
            
            // TODO: Optimize by using FromJsonOverwrite?
            var profileData = File.ReadAllText(_profilePath);
            var playerProfile = JsonUtility.FromJson<LocalPlayerProfile>(profileData);

            var existingDeck = playerProfile.Decks.FirstOrDefault(d => d.Id == updatedDeck.Id);
            if (existingDeck != null)
            {
                playerProfile.Decks.Remove(existingDeck);
            }
            
            playerProfile.Decks.Add(updatedDeck);
            
            var serialized = JsonUtility.ToJson(playerProfile, prettyPrint: true);
            File.WriteAllText(_profilePath, serialized);
            
            Debug.Log($"Saved player deck to {_profilePath}.");
        }
    }
}