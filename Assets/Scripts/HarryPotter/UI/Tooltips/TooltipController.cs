using System;
using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.UI.Tooltips
{
    public class TooltipController : MonoBehaviour
    {
        public RectTransform TooltipContainer;
        public TextMeshProUGUI DescriptionText;
        public TextMeshProUGUI ActionText;
        
        private GameViewSystem _gameView;
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;

        private RectTransform _descriptionContainer;
        private RectTransform _actionContainer;

        private static readonly Vector2 LeftPivot = new Vector2(0f, 0f);
        private static readonly Vector2 RightPivot = new Vector2(1f, 0f);
        
        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _canvas = GetComponent<Canvas>();
            _canvasScaler = _canvas.GetComponent<CanvasScaler>();
            _descriptionContainer = DescriptionText.rectTransform.parent.GetComponent<RectTransform>();
            _actionContainer = ActionText.rectTransform.parent.GetComponent<RectTransform>();
            Hide();
        }

        private void Update()
        {
            TooltipContainer.position = UnityEngine.Input.mousePosition;

            //NOTE: The TooltipContainer's x position is center-aligned, so divide the reference resolution by 2. 
            if (TooltipContainer.anchoredPosition.x + TooltipContainer.sizeDelta.x > _canvasScaler.referenceResolution.x / 2f)
            {
                TooltipContainer.SetPivot(RightPivot);
            }
            else
            {
                TooltipContainer.SetPivot(LeftPivot);
            }
        }

        public void Show(ITooltipContent content)
        {
            DescriptionText.text = content.GetDescriptionText();
            _descriptionContainer.gameObject.SetActive(true);

            ActionText.text = content.GetActionText();

            if (!string.IsNullOrEmpty(ActionText.text))
            {
                _actionContainer.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            _descriptionContainer.gameObject.SetActive(false);
            _actionContainer.gameObject.SetActive(false);
        }
    }
}