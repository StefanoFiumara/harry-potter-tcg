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
        public List<ZoneUI> Zones;

        private void Awake()
        {
            Zones = GetComponentsInChildren<ZoneUI>().ToList();

            var allZones = Enum.GetValues(typeof(Zone)).Cast<Zone>().ToList();
            var assignedZones = Zones.Select(s => s.Zone).ToList();

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
            foreach (var cardData in PlayerState.StartingDeck)
            {
                // TEMP: Spawn cards in random zones to check how they are working
                var zones = Enum.GetValues(typeof(Zone)).Cast<Zone>().ToList();
                var randomZone = zones[UnityEngine.Random.Range(0, zones.Count)];
                SpawnCard(cardData, randomZone);
            }
        }

        private void SpawnCard(CardData cardData, Zone zone)
        {
            var targetZone = ZoneFor(zone);

            var position = targetZone.GetNextPosition();
            var rotation = targetZone.GetTargetRotation();

            var state = new CardState(cardData) { Zone = zone };

            var card = Instantiate(CardPrefab, position, rotation, targetZone.transform);
            card.Init(cardData, state);

            card.gameObject.name = card.Data.CardName;
            Cards.Add(card);

            PlayerState.Cards.Add(state);
        }

        private ZoneUI ZoneFor(Zone zone) => Zones.Single(z => z.Zone == zone);
    }
}
