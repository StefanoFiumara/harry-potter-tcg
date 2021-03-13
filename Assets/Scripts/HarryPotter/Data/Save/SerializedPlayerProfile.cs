using System;
using System.Collections.Generic;

namespace HarryPotter.Data.Save
{
    [Serializable]
    public class SerializedPlayerProfile
    {
        public string ProfileName;
        public string SelectedDeckId;
        public List<SerializedDeck> Decks;
    }
    
    [Serializable]
    public class SerializedDeck
    {
        public string Id;
        public string Name;
        public string StartingCharacterId;
        public List<string> Cards;
    }
}