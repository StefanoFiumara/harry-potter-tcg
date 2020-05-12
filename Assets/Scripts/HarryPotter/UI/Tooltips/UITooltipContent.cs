using System;
using HarryPotter.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace HarryPotter.UI.Tooltips
{
    public class UITooltipContent : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler
    {
        [FormerlySerializedAs("TooltipText")] 
        [TextArea]
        public string DescriptionText;

        [TextArea] 
        public string ActionText;
        
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

        public string GetDescriptionText()
        {
            return DescriptionText;
        }

        public string GetActionText()
        {
            return ActionText;
        }
    }
}