using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Views.UI.Tooltips
{
    public class UITooltipContent : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea]
        public string DescriptionText;

        [TextArea] 
        public string ActionText;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Global.Tooltip.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Global.Tooltip.Hide();
        }

        public string GetDescriptionText() => DescriptionText;

        public string GetActionText(MonoBehaviour context = null) => ActionText;
    }
}