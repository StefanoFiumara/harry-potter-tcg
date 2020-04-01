using System.Text;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Systems;
using TMPro;
using UnityEngine;
using Utils;

namespace HarryPotter.UI
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

        public void Show(Card c)
        {
            var tooltipText = new StringBuilder();

            var lessonCost = c.GetAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount}  {lessonCost.Type.IconText()}</align>");
            }
            tooltipText.AppendLine($"<b>{c.Data.CardName}</b>");
            tooltipText.AppendLine($"<i>{c.Data.Type}</i>");

            if (!string.IsNullOrWhiteSpace(c.Data.CardDescription))
            {
                tooltipText.AppendLine(c.Data.CardDescription);                
            }
            
            Text.text = tooltipText.ToString();
            
            Panel.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Panel.gameObject.SetActive(false);
        }

        private void FollowMousePosition()
        {
            float width = Screen.width*Panel.anchorMin.x;
            float height = Screen.height*Panel.anchorMin.y;

            float xoffset = 0;
            float yoffset = 0;

            var mousePos = UnityEngine.Input.mousePosition;
            
            if(Screen.width > 1024) {
                float difference = Screen.width-1024;
                float percentage = (mousePos.x/(float)Screen.width)*100;
                xoffset = (percentage*difference)/100.0f;
            }
            if(Screen.height > 768) {
                float difference = Screen.height-768;
                float percentage = ((float)(Screen.height-mousePos.y)/(float)Screen.height)*100;
                yoffset = (percentage*difference)/100.0f;
            }

            if(Screen.width < 1024) {
                float difference = 1024-Screen.width;
                float percentage = (mousePos.x/(float)Screen.width)*100;
                xoffset = -(percentage*difference)/100.0f;
            }

            if(Screen.height < 768) {
                float difference = 768-Screen.height;
                float percentage = ((float)(Screen.height-mousePos.y)/(float)Screen.height)*100;
                yoffset = -(percentage*difference)/100.0f;
            }  

            Panel.anchoredPosition = new Vector2(mousePos.x-width-xoffset,mousePos.y-height+yoffset);
        }
    }
}