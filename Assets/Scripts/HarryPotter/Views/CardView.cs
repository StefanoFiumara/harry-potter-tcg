using System;
using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.UI.Tooltips;
using TMPro;
using UnityEngine;
using Utils;

namespace HarryPotter.Views
{
    public class CardView : MonoBehaviour, ITooltipContent
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
                _gameView.Tooltip.Show(this);
            }
        }

        private void OnMouseExit()
        {
            _gameView.Tooltip.Hide();
        }

        public string GetTooltipText()
        {
            var tooltipText = new StringBuilder();

            var lessonCost = _card.GetAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount}  {lessonCost.Type.IconText()}</align>");
            }
            tooltipText.AppendLine($"<b>{_card.Data.CardName}</b>");
            tooltipText.AppendLine($"<i>{_card.Data.Type}</i>");

            if (!string.IsNullOrWhiteSpace(_card.Data.CardDescription))
            {
                tooltipText.AppendLine(_card.Data.CardDescription);                
            }

            return tooltipText.ToString();
        }
    }
}