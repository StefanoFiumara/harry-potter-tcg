using System;
using System.Collections.Generic;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards
{
    // NOTE: Marking this class as Serializable allows unity to display its properties in the Inspector.
    [Serializable]
    public class Card
    {
        public CardData Data;

        public Zones Zone;

        public int OrderOfPlay = int.MaxValue;

        public Player Owner;

        public List<CardAttribute> ModifiedAttributes { get; }

        public Card(CardData data, Player owner, Zones zone = Zones.Deck)
        {
            Data = data;
            Owner = owner;
            Zone = zone;

            ModifiedAttributes = new List<CardAttribute>();
            foreach (var attribute in data.Attributes)
            {
                var cloned = attribute.Clone();

                if (cloned is Ability a)
                {
                    a.Owner = this;
                }
                
                ModifiedAttributes.Add(cloned);
            }
        }
    }
}