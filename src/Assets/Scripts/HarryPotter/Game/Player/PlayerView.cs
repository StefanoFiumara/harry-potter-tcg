using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using UnityEngine;

namespace HarryPotter.Game.Player
{
    public class PlayerView : MonoBehaviour
    {
        public PlayerState PlayerState;
        public CardView CardPrefab;

        public List<CardView> Cards;
        private Dictionary<Zone, ZoneView> _zoneViews;

        private void Awake()
        {
            _zoneViews = GetComponentsInChildren<ZoneView>().ToDictionary(z => z.Zone, z => z);

            var allZones = Enum.GetValues(typeof(Zone)).Cast<Zone>().ToList();
            var assignedZones = _zoneViews.Keys.ToList();

            if (assignedZones.Distinct().Count() != allZones.Count)
            {
                var missing = allZones.Except(assignedZones).ToList();
                var message = string.Join(", ", missing);
                
                throw new UnityException($"PlayerView does not have all Zones assigned! Make sure Zones are properly assigned for all Zone Views.\nMissing Zones: {message}");
            }

            if (assignedZones.GroupBy(z => z).Any(g => g.Count() > 1))
            {
                throw new UnityException("PlayerView has multiple views defined for each zone.");
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
            var targetZone = _zoneViews[zone];

            var position = targetZone.GetNextPosition();
            var rotation = targetZone.GetTargetRotation();

            var state = new CardState(cardData) { Zone = zone };

            var card = Instantiate(CardPrefab, position, rotation, targetZone.transform);
            card.Init(cardData, state, this);

            card.gameObject.name = card.Data.CardName;
            Cards.Add(card);

            PlayerState.Cards.Add(state);
        }

        public void MoveToZone(CardView card, Zone targetZone)
        {
            var previous = card.State.Zone;
            card.State.Zone = targetZone;

            RepositionZones(previous, targetZone);
        }

        private void RepositionZones(params Zone[] zones)
        {
            if(!zones.Any()) return;

            var zonesToReposition = _zoneViews.Where(kvp => zones.Contains(kvp.Key)).Select(kvp => kvp.Value);

            foreach (var container in zonesToReposition)
            {
                var cardsInZone = Cards.Where(c => c.State.Zone == container.Zone).ToList();

                for (int i = 0; i < cardsInZone.Count; i++)
                {
                    var card = cardsInZone[i];
                    var position = container.GetPositionForIndex(i);
                    var rotation = container.GetTargetRotation(); // for index?

                    //TODO: Set up Animations/Tweening between zones.
                    card.transform.position = position;
                    card.transform.rotation = rotation;
                }
            }
        }
    }
}
