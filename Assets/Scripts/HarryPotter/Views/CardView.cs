using System;
using System.Linq;
using System.Text;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Input.InputStates;
using HarryPotter.Systems;
using HarryPotter.UI;
using HarryPotter.UI.Tooltips;
using HarryPotter.Utils;
using TMPro;
using UnityEngine;

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
        private Lazy<string> _toolTipDescription;
        
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
            
            _toolTipDescription =  new Lazy<string>(GetToolTipDescription);
        }

        private void InitView(Card c)
        {
            CardFaceRenderer.sprite = c.Data.Image;
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
                
                tooltipText.AppendLine(_toolTipDescription.Value);                
            }

            return tooltipText.ToString();
        }

        public string GetActionText(MonoBehaviour context)
        {
            if (_gameView.IsIdle &&
                _gameView.Input.StateMachine.CurrentState is ITooltipContent tc)
            {
                return tc.GetActionText(this);
            }

            return string.Empty;
        }

        public void Highlight(Color color)
        {
            CardFaceRenderer.color = color == Color.clear ? Color.white : color;
        }
        
        private string GetToolTipDescription()
        {
            const int wordsPerLine = 12;

            var words = _card.Data.CardDescription.Split(' ');
            var splitText = new StringBuilder();

            int wordCount = 0;

            foreach (string word in words)
            {
                splitText.Append($"{word} ");
                wordCount++;

                if (wordCount > wordsPerLine)
                {
                    splitText.AppendLine();
                    wordCount = 0;
                }
            }

            return splitText.ToString().TrimEnd(' ', '\n');
        }
    }
}