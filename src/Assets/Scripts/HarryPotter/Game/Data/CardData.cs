using System.Collections.Generic;
using HarryPotter.Enums;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using Utils;

namespace HarryPotter.Game.Data
{
    public class CardData : ScriptableObject
    {
        public string CardName; //Really for logging purposes only
        
        [Space(10)]
        [EnumFlags]
        public Tag Tags;
        
        [HideInInspector]
        public Sprite Image;
        
        [HideInInspector]
        public List<PlayCondition> PlayConditions = new List<PlayCondition>();

        [HideInInspector] 
        public List<CardAttribute> Attributes = new List<CardAttribute>();

        [HideInInspector] 
        public List<PlayAction> PlayActions = new List<PlayAction>();
        
        [HideInInspector]
        public CardType Type;
    }
}

