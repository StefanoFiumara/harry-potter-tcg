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