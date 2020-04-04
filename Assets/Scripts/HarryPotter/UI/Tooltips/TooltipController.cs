using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using TMPro;
using UnityEngine;
using Utils;

namespace HarryPotter.UI.Tooltips
{
    public class TooltipController : MonoBehaviour
    {
        public RectTransform Panel;
        public TextMeshProUGUI Text;
        
        private GameViewSystem _gameView;
        private Canvas _canvas;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _canvas = GetComponent<Canvas>();
            Hide();
        }

        private void Update()
        {
            Panel.position = UnityEngine.Input.mousePosition;
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