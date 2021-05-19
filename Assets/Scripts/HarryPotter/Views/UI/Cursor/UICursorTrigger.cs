using System;
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
        
        private GameView _gameView;

        private Button _button;
        
        private void Awake()
        {
            _gameView = GetComponentInParent<GameView>();
            _button = GetComponent<Button>();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button.IsInteractable() && (_gameView == null || _gameView.IsIdle))
            {
                Global.Cursor.SetCursor(CursorToShow);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Global.Cursor.ResetCursor();
        }

        private void OnDisable()
        {
            if (Global.Cursor != null)
            {
                Global.Cursor.ResetCursor();
            }
            
        }
    }
}