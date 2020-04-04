using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace HarryPotter.UI.Tooltips
{
    public class TooltipController : MonoBehaviour
    {
        public RectTransform Panel;
        public TextMeshProUGUI Text;
        
        private GameViewSystem _gameView;
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;

        private static readonly Vector2 LeftPivot = new Vector2(0f, 0f);
        private static readonly Vector2 RightPivot = new Vector2(1f, 0f);
        
        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _canvas = GetComponent<Canvas>();
            _canvasScaler = _canvas.GetComponent<CanvasScaler>();
            Hide();
        }

        private void Update()
        {
            Panel.position = UnityEngine.Input.mousePosition;

            //NOTE: The Panel's x position is center-aligned, so divide the reference resolution by 2. 
            if (Panel.anchoredPosition.x + Panel.sizeDelta.x > _canvasScaler.referenceResolution.x / 2f)
            {
                Panel.SetPivot(RightPivot);
            }
            else
            {
                Panel.SetPivot(LeftPivot);
            }
        }

        public void Show(ITooltipContent content)
        {
            Text.text = content.GetTooltipText();
            
            Panel.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Panel.gameObject.SetActive(false);
        }
    }
}