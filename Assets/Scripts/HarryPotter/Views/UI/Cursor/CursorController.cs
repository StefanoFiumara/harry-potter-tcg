using HarryPotter.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.Views.UI.Cursor
{
    public class CursorController : MonoBehaviour
    {
        public RectTransform CursorRect;
        public Image CursorImage;
        
        public Sprite DefaultCursor;
        public Sprite ActionCursor;

        private void Awake()
        {
            UnityEngine.Cursor.visible = false;
        }

        private void Update()
        {
            CursorRect.position = UnityEngine.Input.mousePosition;
        }

        public void SetCursor(Sprite cursorSprite)
        {
            if (CursorImage != null)
            {
                CursorImage.sprite = cursorSprite;                
            }
        }
        
        public void SetActionCursor()
        {
            SetCursor(ActionCursor);
        }

        public void ResetCursor()
        {
            SetCursor(DefaultCursor);
        }
    }
}