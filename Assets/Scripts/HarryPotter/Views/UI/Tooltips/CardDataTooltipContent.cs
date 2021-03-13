using HarryPotter.Data.Cards;
using HarryPotter.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Views.UI.Tooltips
{
    public class CardDataTooltipContent : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler
    {
        public CardData CardData;
        
        private TooltipController _controller;
        
        private void Awake()
        {
            _controller = FindObjectOfType<TooltipController>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _controller.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _controller.Hide();
        }

        public string GetDescriptionText() => CardData.GetFormattedTooltipText();

        public string GetActionText(MonoBehaviour context = null) => string.Empty;
    }
}