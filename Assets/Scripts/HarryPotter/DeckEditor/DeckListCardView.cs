using HarryPotter.Data.Cards;
using HarryPotter.Views.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.DeckEditor
{
    public class DeckListCardView : LibraryCardView
    {
        public TMP_Text CardName;
        private int _count;

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                CardName.text = $"x{value} {Data.CardName}";    
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            EditorView.RemoveCardFromDeck(this);
        }

        public override string GetActionText(MonoBehaviour context)
        {
            return $"{TextIcons.MOUSE_LEFT}/{TextIcons.MOUSE_RIGHT} Remove from deck";
        }

        public override void InitView(CardData c, int count = 1)
        {
            base.InitView(c, count);
            Count = count;
        }
    }
}