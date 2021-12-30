using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Card Library")]
    public class CardLibrary : ScriptableObject
    {
        // ReSharper disable once InconsistentNaming
        [SerializeField]
        private List<CardData> _Cards;

        public void UpdateCards(List<CardData> updatedCards)
        {
            _Cards = updatedCards;
        }
        
        public List<CardData> Cards => _Cards.Where(c => c.IsHqGraphics).ToList();

        public CardData GetById(string id) => _Cards.SingleOrDefault(c => c.Id == id);
    }
}