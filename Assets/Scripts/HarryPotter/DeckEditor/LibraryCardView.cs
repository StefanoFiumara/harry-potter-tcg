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
    public class LibraryCardView : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image CardFaceRenderer;

        public CardData Data { get; private set; }
        
        private Lazy<string> _toolTipDescription;
        protected DeckBuilderView EditorView;
        
        protected virtual void Awake()
        {
            EditorView = GetComponentInParent<DeckBuilderView>();
            _toolTipDescription =  new Lazy<string>(GetToolTipDescription);
        }

        public virtual void InitView(CardData c, int count = 1)
        {
            Data = c;
            CardFaceRenderer.sprite = c.Image;
        }

        
        public void OnPointerEnter(PointerEventData data)
        {
            EditorView.Tooltip.Show(this);
            
            if (true) // TODO: if(can-be-added-to-deck)
            {
                EditorView.Cursor.SetActionCursor();
                // TODO: Particles?
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            EditorView.Tooltip.Hide();
            EditorView.Cursor.ResetCursor();
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                EditorView.AddCardToDeck(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                EditorView.SetStartingCharacter(this);
            }
        }

        public string GetDescriptionText()
        {
            // TODO: Almost the same as card view, just does not show creature's max health, consolidate?
            var tooltipText = new StringBuilder();

            var lessonCost = Data.GetDataAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount} {TextIcons.FromLesson(lessonCost.Type)}</align>");
            }
            
            tooltipText.AppendLine($"<b>{Data.CardName}</b>");
            
            tooltipText.AppendLine($"<i>{Data.Type}</i>");
            if (Data.Tags != Tag.None)
            {
                tooltipText.AppendLine($"<size=10>{string.Join(" * ", Data.Tags)}</size>");
            }
            

            var creature = Data.GetDataAttribute<Creature>();
            if (creature != null)
            {
                tooltipText.AppendLine($"{TextIcons.ICON_ATTACK} {creature.Attack}");
                tooltipText.AppendLine($"{TextIcons.ICON_HEALTH} {creature.Health}");
            }
            
            if (!string.IsNullOrWhiteSpace(Data.CardDescription))
            {
                
                tooltipText.AppendLine(_toolTipDescription.Value);                
            }

            return tooltipText.ToString();
        }

        public virtual string GetActionText(MonoBehaviour context)
        {
            var actionText = $"{TextIcons.MOUSE_LEFT} Add to Deck";

            if (Data.Type == CardType.Character && Data.Tags.HasTag(Tag.Witch | Tag.Wizard))
            {
                actionText += $" - {TextIcons.MOUSE_RIGHT} Set Starting Character";
            }
            
            return actionText;
        }

        public void Highlight(Color color)
        {
           // TODO: highlight?
        }
        
        private string GetToolTipDescription()
        {
            // TODO: Repeated code from CardView, consolidate into a helper function
            const int wordsPerLine = 6;

            var words = Data.CardDescription.Split(' ');
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