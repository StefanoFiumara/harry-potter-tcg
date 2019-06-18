using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace HarryPotter.Game.Player
{
    public class PlayerView : MonoBehaviour
    {
        public PlayerState PlayerState;
        public CardView CardPrefab;

        private Dictionary<Zone, ZoneView> _zoneViews;
        private Transform _cardContainer;

        private void Awake()
        {
            _zoneViews = GetComponentsInChildren<ZoneView>().ToDictionary(z => z.Zone, z => z);

            _cardContainer = transform.Find("Cards");

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
            foreach (var zone in _zoneViews.Values)
            {
                zone.Cards.Clear();
            }

            // Spawn Deck!
            foreach (var cardData in PlayerState.StartingDeck)
            {
                SpawnCard(cardData, Zone.Deck);
            }
        }

        //TODO: Figure out if special methods like this one should go to the ZoneView instead of PlayerView
        private CardView GetRandomCard(Zone from)
        {
            var random = Random.Range(0, _zoneViews[from].Cards.Count);

            return _zoneViews[from].Cards[random];
        }

        private void SpawnCard(CardData cardData, Zone zone)
        {
            var toZone = _zoneViews[zone];

            var position = toZone.GetNextPosition();
            var rotation = toZone.GetTargetRotation();

            var state = new CardState(cardData) { Zone = zone };

            var card = Instantiate(CardPrefab, position, Quaternion.Euler(rotation), _cardContainer);
            card.Init(cardData, state, this);

            card.gameObject.name = card.Data.CardName;

            toZone.Cards.Add(card);
            PlayerState.Cards.Add(state); //TODO: I'm starting to think this may not be necessary...
        }

        public Sequence MoveToZone(CardView card, Zone targetZone) // with preview?
        {
            if (card.State.Zone == targetZone) return DOTween.Sequence();

            var previousZone = card.State.Zone;
            
            var position = _zoneViews[targetZone].GetNextPosition();
            var rotation = _zoneViews[targetZone].GetTargetRotation();

            var sequence = card.Move(position, rotation);

            card.State.Zone = targetZone;

            _zoneViews[previousZone].Cards.Remove(card);
            _zoneViews[targetZone].Cards.Add(card);

            RepositionZones(previousZone);

            return sequence;
        }

        private void RepositionZones(params Zone[] zones)
        {
            if(!zones.Any()) return;

            var zonesToReposition = _zoneViews.Where(kvp => zones.Contains(kvp.Key)).Select(kvp => kvp.Value);

            foreach (var zone in zonesToReposition)
            {
                for (int i = 0; i < zone.Cards.Count; i++)
                {
                    var card = zone.Cards[i];
                    var position = zone.GetPositionForIndex(i);
                    var rotation = zone.GetTargetRotation(); // for index?

                    card.Move(position, rotation);
                }
            }
        }
    }
}
