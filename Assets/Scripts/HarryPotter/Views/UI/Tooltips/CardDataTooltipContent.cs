using HarryPotter.Data.Cards;
using HarryPotter.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Views.UI.Tooltips
{
    public class CardDataTooltipContent : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler
    {
        public CardData CardData;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Global.Tooltip.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Global.Tooltip.Hide();
        }

        public string GetDescriptionText() => CardData.GetFormattedTooltipText();

        public string GetActionText(MonoBehaviour context = null) => string.Empty;
    }
}