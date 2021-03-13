using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarryPotter.Data.Cards;
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
                
                var newPlayerProfile = new SerializedPlayerProfile
                {
                    ProfileName = "New Player",
                    SelectedDeckId = newDeckId,
                    Decks = new List<SerializedDeck>
                    {
                        // TEMP: Create a default deck slot for the player, this can probably go away when we implement multiple deck management
                        new SerializedDeck
                        {
                            Id = newDeckId,  
                            Name = "Default Deck", 
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
            var playerProfile = JsonUtility.FromJson<SerializedPlayerProfile>(profileData);

            // TODO: Is deckToLoad ever going to be null?
            var serializedDeck = playerProfile.Decks.FirstOrDefault(d => d.Id == playerProfile.SelectedDeckId) ?? playerProfile.Decks.First();

            LocalPlayer.PlayerName = playerProfile.ProfileName;
            
            
            // TODO: Error handling when GetById fails ... Do we need to denote the game version in each save file to ensure these are valid?
            LocalPlayer.SelectedDeck = Deck.Load(serializedDeck, Library);
        }

        public void SaveData()
        {
            var deckId = LocalPlayer.SelectedDeck.DeckId;
            var deckName = LocalPlayer.SelectedDeck.DeckName;
            var startingCharacter = LocalPlayer.SelectedDeck.StartingCharacter.Id;

            var cards = LocalPlayer.SelectedDeck.Cards.Select(c => c.Id).ToList();

            var updatedDeck = new SerializedDeck
            {
                Id = deckId,
                Name = deckName,
                StartingCharacterId = startingCharacter,
                Cards = cards
            };
            
            // TODO: Optimize by using FromJsonOverwrite?
            var profileData = File.ReadAllText(_profilePath);
            var playerProfile = JsonUtility.FromJson<SerializedPlayerProfile>(profileData);

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