using System;
using System.Collections.Generic;

namespace HarryPotter.Data.Save
{
    [Serializable]
    public class LocalPlayerProfile
    {
        public string ProfileName;
        public string SelectedDeckId;
        public List<Deck> Decks;
    }
    
    [Serializable]
    public class Deck
    {
        public string Id;
        public string Name;
        public string StartingCharacterId;
        public List<string> Cards;
    }
}