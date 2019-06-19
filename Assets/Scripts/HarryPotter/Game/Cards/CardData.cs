using System.Collections.Generic;
using HarryPotter.Enums;
using UnityEngine;
using Utils;

namespace HarryPotter.Game.Cards
{
    public class CardData : ScriptableObject
    {
        [HideInInspector]
        public string Id;

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
        public List<CardAction> PlayActions = new List<CardAction>();

        [HideInInspector]
        public List<CardAction> InPlayBeforeTurnActions = new List<CardAction>();

        [HideInInspector]
        public List<CardAction> InPlayAfterTurnActions = new List<CardAction>();

        [HideInInspector]
        public List<CardAction> EnterPlayActions = new List<CardAction>();

        [HideInInspector]
        public List<CardAction> ExitPlayActions = new List<CardAction>();

        [HideInInspector]
        public List<PlayCondition> ActivateConditions = new List<PlayCondition>();

        [HideInInspector]
        public List<CardAction> ActivateActions = new List<CardAction>();

        [HideInInspector]
        public List<CardAction> Reactions = new List<CardAction>();

        [HideInInspector]
        public CardType Type;
    }
}

