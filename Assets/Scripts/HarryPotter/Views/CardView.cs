using System;
using System.Linq;
using System.Text;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Input.InputStates;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using TMPro;
using UnityEngine;

namespace HarryPotter.Views
{
    public class CardView : MonoBehaviour, ITooltipContent
    {
        public SpriteRenderer CardFaceRenderer;

        public ParticleSystem HighlightParticles;
        public ParticleSystem PlayableParticles;

        public TMP_Text TargetCounter;
        
        private Card _card;
        private GameView _gameView;
        private MatchData _match;
        private CardSystem _cardSystem;
        private Lazy<string> _toolTipDescription;

        // TODO: Make configurable in options menu
        private const KeyCode PREVIEW_KEY = KeyCode.LeftShift;

        public Card Card
        {
            get => _card;
            set
            {
                _card = value;
                InitView(_card);
            }
        }

        /// <summary>
        /// Shortcut to get the card's index within the zone it's in.
        /// </summary>
        public int ZoneIndex => _card.Owner[_card.Zone]?.IndexOf(_card) ?? -1;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameView>();
            _match = _gameView.Match;
            _cardSystem = _gameView.Container.GetSystem<CardSystem>();

            _toolTipDescription =  new Lazy<string>(GetToolTipDescription);
            
            PlayableParticles.Stop();
            HighlightParticles.Stop();
            
            HideTargetCounter();
        }

        private void InitView(Card c)
        {
            CardFaceRenderer.sprite = c.Data.Image;
        }

        public void SetSortingLayer(int layer)
        {
            CardFaceRenderer.sortingOrder = layer;
        }
        
        private bool IsInTargetingZone()
        {
            if (_gameView.Input.StateMachine.CurrentState is BaseTargetingState targetState)
            {
                return targetState.IsCandidateZone(_card);
            }

            return false;
        }
        
        private void OnMouseOver()
        {
            var playerOwnsCard = Card.Owner.Index == _gameView.Match.LocalPlayer.Index;
            var cardInHand = Card.Zone == Zones.Hand;
            var isPreview = _gameView.Input.StateMachine.CurrentState is PreviewState;
            var isTargeting = _gameView.Input.StateMachine.CurrentState is BaseTargetingState;
            
            if((playerOwnsCard && cardInHand) || Card.Zone.IsInPlay() || IsInTargetingZone())
            {
                Global.Tooltip.Show(this);
            }

            if (_cardSystem.IsPlayable(Card) && _match.CurrentPlayerIndex == _match.LocalPlayer.Index)
            {
                Global.Cursor.SetActionCursor();
            }

            var hasActivateEffect = _card.GetAbilities(AbilityType.ActivateEffect).Any();
            
            var highlightColor = CalculateHighlightColor();
            
            if (playerOwnsCard && 
                !isPreview && 
                !isTargeting && 
                (cardInHand || Card.Zone.IsInPlay() && hasActivateEffect))
            {
                PlayableParticles.SetParticleColor(highlightColor);
                PlayableParticles.Play();
            }
            else
            {
                PlayableParticles.Stop();
            }
        }

        private Color CalculateHighlightColor()
        {
            var highlightColor =
                _cardSystem.IsPlayable(Card) ? Colors.Playable
                : _cardSystem.IsActivatable(Card) ? Colors.Activatable
                : Colors.Unplayable;
            
            return highlightColor;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(PREVIEW_KEY))
            {
                var playerOwnsCard = Card.Owner.Index == _gameView.Match.LocalPlayer.Index;
                var cardInHand = Card.Zone == Zones.Hand;
                var cardInPlay = Card.Zone.IsInPlay();
                
                if (playerOwnsCard && (cardInHand || cardInPlay))
                {
                    var highlightColor = CalculateHighlightColor();
                    
                    PlayableParticles.SetParticleColor(highlightColor);
                    PlayableParticles.Play();
                }
            }
            else if (UnityEngine.Input.GetKeyUp(PREVIEW_KEY))
            {
                PlayableParticles.Stop();
            }
        }

        private void OnMouseExit()
        {
            Global.Tooltip.Hide();
            Global.Cursor.ResetCursor();

            if (!UnityEngine.Input.GetKey(PREVIEW_KEY))
            {
                PlayableParticles.Stop();
            }
            
        }

        // TODO: Consolidate in GetFormattedTooltipText extension
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
            if (_card.Data.Tags != Tag.None)
            {
                tooltipText.AppendLine($"<size=10>{string.Join(" * ", _card.Data.Tags)}</size>");
            }
            

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

            var provider = _card.GetAttribute<LessonProvider>();
            if (provider != null)
            {
                var icon = TextIcons.FromLesson(provider.Type);
                var icons = string.Join(" ", Enumerable.Repeat(icon, provider.Amount));
                tooltipText.AppendLine($"\nProvides {icons}");
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
            if (color == Color.clear)
            {
                HighlightParticles.Stop();
            }
            else
            {
                HighlightParticles.SetParticleColor(color);
                HighlightParticles.Play();
            }
        }

        public void SetTargetCounter(int number)
        {
            TargetCounter.text = $"{number}";
        }

        public void HideTargetCounter()
        {
            TargetCounter.text = string.Empty;
        }
        
        private string GetToolTipDescription()
        {
            const int wordsPerLine = 6;

            var words = _card.Data.CardDescription.Split(' ');
            var splitText = new StringBuilder();

            var wordCount = 0;

            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                splitText.Append($"{word} ");

                // NOTE: Quick hack to prevent line breaks from being inserted inside <sprite> tags.
                if (word.StartsWith("<"))
                {
                    //IMPORTANT: Does not account for tags not separated by a space, does this matter?
                    while (!word.EndsWith(">"))
                    {
                        i++;
                        word = words[i];
                        splitText.Append($"{word} ");
                    }
                }
                
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