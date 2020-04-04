using HarryPotter.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.UI.Cursor
{
    public class CursorController : MonoBehaviour
    {
        public RectTransform CursorRect;
        
        public Image CursorImage;
        
        public Sprite DefaultCursor;
        
        public Sprite ActionCursor;

        
        private GameViewSystem _gameView;

        private void Awake()
        {
            UnityEngine.Cursor.visible = false;
            _gameView = GetComponentInParent<GameViewSystem>();
            
        }

        private void Update()
        {
            CursorRect.position = UnityEngine.Input.mousePosition;
        }

        public void SetCursor(Sprite cursorSprite)
        {
            CursorImage.sprite = cursorSprite;
        }
        
        public void SetActionCursor()
        {
            SetCursor(ActionCursor);
        }

        public void ResetCursor()
        {
            SetCursor(DefaultCursor);
        }
        
        private void OnDestroy()
        {
            UnityEngine.Cursor.visible = true;
        }
    }
}