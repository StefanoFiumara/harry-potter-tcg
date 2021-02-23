using System;
using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Utils;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HarryPotter.DeckEditor
{
    public class DeckBuilderCardView : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler
    {
        public Image CardFaceRenderer;
        
        private CardData _card;
        
        private Lazy<string> _toolTipDescription;
        private DeckBuilderView _editorView;
        
        private void Awake()
        {
            _editorView = GetComponentInParent<DeckBuilderView>();
            _toolTipDescription =  new Lazy<string>(GetToolTipDescription);
        }

        public void InitView(CardData c)
        {
            _card = c;
            CardFaceRenderer.sprite = c.Image;
        }

        
        public void OnPointerEnter(PointerEventData data)
        {
            _editorView.Tooltip.Show(this);
            
            if (true) // TODO: if(can-be-added-to-deck)
            {
                _editorView.Cursor.SetActionCursor();
                // TODO: Particles?
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            _editorView.Tooltip.Hide();
            _editorView.Cursor.ResetCursor();
        }

        // TODO: Almost the same as card view, just does not show creature's max health, consolidate?
        public string GetDescriptionText()
        {
            var tooltipText = new StringBuilder();

            var lessonCost = _card.GetDataAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount} {TextIcons.FromLesson(lessonCost.Type)}</align>");
            }
            
            tooltipText.AppendLine($"<b>{_card.CardName}</b>");
            
            tooltipText.AppendLine($"<i>{_card.Type}</i>");
            if (_card.Tags != Tag.None)
            {
                tooltipText.AppendLine($"<size=10>{string.Join(" * ", _card.Tags)}</size>");
            }
            

            var creature = _card.GetDataAttribute<Creature>();
            if (creature != null)
            {
                tooltipText.AppendLine($"{TextIcons.ICON_ATTACK} {creature.Attack}");
                tooltipText.AppendLine($"{TextIcons.ICON_HEALTH} {creature.Health}");
            }
            
            if (!string.IsNullOrWhiteSpace(_card.CardDescription))
            {
                
                tooltipText.AppendLine(_toolTipDescription.Value);                
            }

            return tooltipText.ToString();
        }

        public string GetActionText(MonoBehaviour context)
        {
            return string.Empty;
        }

        public void Highlight(Color color)
        {
           // TODO: highlight
        }
        
        // TODO: Repeated code from CardView
        private string GetToolTipDescription()
        {
            const int wordsPerLine = 6;

            var words = _card.CardDescription.Split(' ');
            var splitText = new StringBuilder();

            int wordCount = 0;

            for (var i = 0; i < words.Length; i++)
            {
                string word = words[i];
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