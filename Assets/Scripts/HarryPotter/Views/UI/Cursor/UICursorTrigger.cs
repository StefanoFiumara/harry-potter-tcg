using HarryPotter.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HarryPotter.Views.UI.Cursor
{
    [RequireComponent(typeof(Button))]
    public class UICursorTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
    {
        public Sprite CursorToShow;
        
        private GameViewSystem _gameView;

        private CursorController _cursor;
        
        private Button _button;
        
        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _cursor = FindObjectOfType<CursorController>();
            _button = GetComponent<Button>();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button.IsInteractable() && (_gameView == null || _gameView.IsIdle))
            {
                _cursor.SetCursor(CursorToShow);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cursor.ResetCursor();
        }
    }
}