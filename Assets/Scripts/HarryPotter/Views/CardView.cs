using System;
using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.Systems;
using TMPro;
using UnityEngine;

namespace HarryPotter.Views
{
    public class CardView : MonoBehaviour
    {
        public SpriteRenderer CardFaceRenderer;
        public SpriteRenderer CardBackRenderer;

        private Card _card;
        private GameViewSystem _gameView;

        public Card Card
        {
            get => _card;
            set
            {
                _card = value;
                InitView(_card);
            }
        }

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
        }

        private void InitView(Card c)
        {
            CardBackRenderer.sprite = c.Data.Image;
        }

        private void OnMouseOver()
        {
            var playerOwnsCard = Card.Owner.Index == _gameView.Game.CurrentPlayerIndex;
            var cardInHand = Card.Zone == Zones.Hand;

            if (playerOwnsCard && cardInHand || Card.Zone.IsInBoard())
            {
                _gameView.Tooltip.Show(_card);
            }
        }

        private void OnMouseExit()
        {
            _gameView.Tooltip.Hide();
        }
    }
}