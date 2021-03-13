using System.Collections.Generic;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Save;

namespace HarryPotter.Data
{
    public class Deck
    {
        public string DeckId { get; private set; }
        public string DeckName { get; private set; }
        public CardData StartingCharacter { get; set; }
        public List<CardData> Cards { get; } = new List<CardData>();
        
        private Deck() { }

        public static Deck Load(SerializedDeck serializedDeck, CardLibrary library)
        {
            var loadedDeck = new Deck
            {
                DeckId = serializedDeck.Id,
                DeckName = serializedDeck.Name,
                StartingCharacter = library.GetById(serializedDeck.StartingCharacterId)
            };

            foreach (var cardId in serializedDeck.Cards)
            {
                var cardData = library.GetById(cardId);
                loadedDeck.Cards.Add(cardData);
            }

            return loadedDeck;
        }
    }
}