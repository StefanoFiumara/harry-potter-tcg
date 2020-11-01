using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Card Library")]
    public class CardLibrary : ScriptableObject
    {
        public List<CardData> Cards;

        public CardData GetById(string id) => Cards.SingleOrDefault(c => c.Id == id);
    }
}