using HarryPotter.Data.Cards;
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
        
        protected DeckBuilderView EditorView;
        
        protected virtual void Awake()
        {
            EditorView = GetComponentInParent<DeckBuilderView>();
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

        public string GetDescriptionText() => Data.GetFormattedTooltipText();

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
    }
}