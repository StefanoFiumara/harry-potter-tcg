using HarryPotter.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HarryPotter.UI.Cursor
{
    [RequireComponent(typeof(Button))]
    public class UICursorTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
    {
        public Sprite CursorToShow;
        
        private GameViewSystem _gameView;

        private Button _button;
        
        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _button = GetComponent<Button>();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button.IsInteractable() && _gameView.IsIdle)
            {
                _gameView.Cursor.SetCursor(CursorToShow);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _gameView.Cursor.ResetCursor();
        }
    }
}