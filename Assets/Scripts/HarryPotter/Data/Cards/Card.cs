using System;
using System.Collections.Generic;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards
{
    [Serializable] //TODO: I don't remember why this is Serializable
    public class Card
    {
        public CardData Data;
        
        public Zones Zone;

        public int OrderOfPlay = int.MaxValue;

        public int OwnerIndex;
        
        public Card(CardData data)
        {
            Data = data;
            Zone = Zones.Deck;
        }
    }
}