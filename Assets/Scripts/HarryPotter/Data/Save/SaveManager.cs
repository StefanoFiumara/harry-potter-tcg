using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HarryPotter.Data.Save
{
    public class SaveManager : MonoBehaviour
    {
        private string ProfilePath => Path.Combine(Application.persistentDataPath, "hptcg_profile.json");

        public CardLibrary Library;
        public Player LocalPlayer;
        public Player AIPlayer;
        public Player RemotePlayer;

        private void Awake()
        {
            EnsureProfileExists();
            LoadData();
        }

        private void EnsureProfileExists()
        {
            if (File.Exists(ProfilePath))
            {
                return;
            }
            
            Debug.Log($"No player profile found - Creating new player profile in {ProfilePath}");
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
                        Name = "Default",
                        Cards = new List<string> {Library.Cards.First().Id},
                        StartingCharacterId = Library.Cards.First(c => c.CardName == "Hermione Granger").Id,
                    }
                }
            };

            var serialized = JsonUtility.ToJson(newPlayerProfile, prettyPrint: true);
            File.AppendAllText(ProfilePath, serialized);
        }

        public void LoadData()
        {
            var profileData = File.ReadAllText(ProfilePath);
            var playerProfile = JsonUtility.FromJson<SerializedPlayerProfile>(profileData);

            // TODO: Is deckToLoad ever going to be null?
            var serializedDeck = playerProfile.Decks.FirstOrDefault(d => d.Id == playerProfile.SelectedDeckId) ?? playerProfile.Decks.First();

            LocalPlayer.PlayerName = playerProfile.ProfileName;
            
            LocalPlayer.SelectedDeck = Deck.Load(serializedDeck, Library);
            Debug.Log($"Loaded player profile from {ProfilePath}");
            
            // TEMP: Load the AI deck a different way? from a different file?
            AIPlayer.SelectedDeck = Deck.Load(serializedDeck, Library);
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
            var profileData = File.ReadAllText(ProfilePath);
            var playerProfile = JsonUtility.FromJson<SerializedPlayerProfile>(profileData);

            var existingDeck = playerProfile.Decks.FirstOrDefault(d => d.Id == updatedDeck.Id);
            if (existingDeck != null)
            {
                playerProfile.Decks.Remove(existingDeck);
            }
            
            playerProfile.Decks.Add(updatedDeck);
            
            var serialized = JsonUtility.ToJson(playerProfile, prettyPrint: true);
            File.WriteAllText(ProfilePath, serialized);
            
            Debug.Log($"Saved player deck to {ProfilePath}.");
        }
    }
}