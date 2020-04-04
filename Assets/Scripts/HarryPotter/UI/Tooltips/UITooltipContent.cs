using System;
using HarryPotter.Systems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.UI.Tooltips
{
    public class UITooltipContent : MonoBehaviour, ITooltipContent, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea]
        public string TooltipText;

        private GameViewSystem _gameView;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _gameView.Tooltip.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _gameView.Tooltip.Hide();
        }

        public string GetTooltipText()
        {
            return TooltipText;
        }
    }
}