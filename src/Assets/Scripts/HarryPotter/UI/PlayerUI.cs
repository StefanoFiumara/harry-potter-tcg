using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Enums;
using HarryPotter.Game.Data;
using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public PlayerState PlayerState;
        public CardUI CardPrefab;

        public List<CardUI> Cards;
        public Dictionary<Zone, ZoneUI> Zones;

        private void Awake()
        {
            Zones = GetComponentsInChildren<ZoneUI>().ToDictionary(z => z.Zone, z => z);

            var allZones = Enum.GetValues(typeof(Zone)).Cast<Zone>().ToList();
            var assignedZones = Zones.Keys.ToList();

            if (assignedZones.Distinct().Count() != allZones.Count)
            {
                var missing = allZones.Except(assignedZones).ToList();
                var message = string.Join(", ", missing);
                
                throw new UnityException($"PlayerUI does not have all spawn points assigned! Make sure spawn points are defined for all Zones.\nMissing Zones: {message}");
            }

            if (assignedZones.GroupBy(z => z).Any(g => g.Count() > 1))
            {
                throw new UnityException("PlayerUI has multiple spawn points defined for each zone.");
            }
        }
        
        private void Start()
        {
            Cards.Clear();

            // Spawn Deck!
            // TEMP: Spawn cards in random zones to check how they are working
            int zIndex = 0;
            var zones = Enum.GetValues(typeof(Zone)).Cast<Zone>().ToList();
            foreach (var cardData in PlayerState.StartingDeck)
            {                
                var randomZone = zones[zIndex];
                SpawnCard(cardData, randomZone);

                zIndex++;
                if (zIndex >= zones.Count) zIndex = 0;
            }
        }

        private void SpawnCard(CardData cardData, Zone zone)
        {
            var targetZone = Zones[zone];

            var position = targetZone.GetNextPosition();
            var rotation = targetZone.GetTargetRotation();

            var state = new CardState(cardData) { Zone = zone };

            var card = Instantiate(CardPrefab, position, rotation, targetZone.transform);
            card.Init(cardData, state);

            card.gameObject.name = card.Data.CardName;
            Cards.Add(card);

            PlayerState.Cards.Add(state);
        }
    }
}
