using System.Collections.Generic;
using HarryPotter.Enums;
using UnityEngine;
using Utils;

namespace HarryPotter.Data.Cards
{
    public class CardData : ScriptableObject
    {
        [HideInInspector]
        public string Id;

        public string CardName;
        public string CardDescription;
        
        [Space(10)]
        [EnumFlags]
        public Tag Tags;
        
        [HideInInspector]
        public Sprite Image;

        [HideInInspector] 
        public List<CardAttribute> Attributes = new List<CardAttribute>();

        [HideInInspector]
        public CardType Type;
    }
}

