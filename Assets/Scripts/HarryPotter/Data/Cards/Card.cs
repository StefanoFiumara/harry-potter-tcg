using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards
{
    public class Card
    {
        public CardData Data { get; }

        public Zones Zone { get; set; }

        // TODO: When is it appropriate to use this?
        public int OrderOfPlay { get; } = int.MaxValue;

        public Player Owner { get; set; }

        public List<CardAttribute> Attributes { get; }

        public Card(CardData data, Player owner, Zones zone = Zones.Deck)
        {
            Data = data;
            Owner = owner;
            Zone = zone;

            Attributes = new List<CardAttribute>();
            
            foreach (var attribute in data.Attributes)
            {
                var cloned = attribute.Clone();

                cloned.Owner = this;
                
                Attributes.Add(cloned);
            }
        }
    }
}