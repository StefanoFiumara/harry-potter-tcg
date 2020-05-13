using System;
using System.Text;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.UI;
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
        private MatchData _match;
        private CardSystem _cardSystem;
        
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
            _match = _gameView.Match;
            _cardSystem = _gameView.Container.GetSystem<CardSystem>();
        }

        private void InitView(Card c)
        {
            CardBackRenderer.sprite = c.Data.Image;
        }

        private void OnMouseOver()
        {
            var playerOwnsCard = Card.Owner.Index == _gameView.Match.LocalPlayer.Index;
            var cardInHand = Card.Zone == Zones.Hand;

            if (playerOwnsCard && cardInHand || Card.Zone.IsInBoard())
            {
                _gameView.Tooltip.Show(this);
            }

            if (_cardSystem.IsPlayable(Card) && _match.CurrentPlayerIndex == _match.LocalPlayer.Index)
            {
                _gameView.Cursor.SetActionCursor();
            }
        }

        private void OnMouseExit()
        {
            _gameView.Tooltip.Hide();
            _gameView.Cursor.ResetCursor();
        }

        public string GetDescriptionText()
        {
            var tooltipText = new StringBuilder();

            var lessonCost = _card.GetAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount} {TextIcons.FromLesson(lessonCost.Type)}</align>");
            }
            tooltipText.AppendLine($"<b>{_card.Data.CardName}</b>");
            tooltipText.AppendLine($"<i>{_card.Data.Type}</i>");

            var creature = _card.GetAttribute<Creature>();
            if (creature != null)
            {
                //TODO: Show current health in separate color if it does not == MaxHealth 
                tooltipText.AppendLine($"{TextIcons.ICON_ATTACK} {creature.Attack}");
                tooltipText.AppendLine($"{TextIcons.ICON_HEALTH} {creature.Health} / {creature.MaxHealth}");
            }
            
            if (!string.IsNullOrWhiteSpace(_card.Data.CardDescription))
            {
                //TODO: Should description text be smaller?
                tooltipText.AppendLine(_card.Data.CardDescription);                
            }

            return tooltipText.ToString();
        }

        public string GetActionText()
        {
            if (_gameView.Input.IsCardPreview && _gameView.Input.ActiveCard != this)
            {
                return string.Empty;
            }
            
            if (!_gameView.IsIdle) return string.Empty;
            
            var tooltipText = new StringBuilder();

            if (_gameView.Input.IsCardPreview)
            {
                tooltipText.AppendLine($"{TextIcons.MOUSE_RIGHT} Back");
            }
            else
            {
                if (_cardSystem.IsPlayable(Card) && _match.CurrentPlayerIndex == _match.LocalPlayer.Index)
                {
                    tooltipText.Append($"{TextIcons.MOUSE_LEFT} Play - ");
                }
                    
                tooltipText.AppendLine($"{TextIcons.MOUSE_RIGHT} View");
            }

            return tooltipText.ToString();
        }
    }
}