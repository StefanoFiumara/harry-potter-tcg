using System;
using HarryPotter.Data.Cards;
using UnityEngine;

namespace HarryPotter.Views
{
    public class CardView : MonoBehaviour
    {
        public SpriteRenderer CardFaceRenderer;
        public SpriteRenderer CardBackRenderer;
        
        private Card _card;
        public Card Card
        {
            get => _card;
            set
            {
                _card = value;
                InitView(_card);
            }
        }

        private void InitView(Card c)
        {
            CardBackRenderer.sprite = c.Data.Image;
            
            //TODO: Tooltip values could be populated here from CardData Name/Description
        }
    }
}