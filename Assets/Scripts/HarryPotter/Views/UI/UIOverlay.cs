using HarryPotter.Data.Save;
using HarryPotter.Views.UI.Cursor;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;

namespace HarryPotter.Views.UI
{
    public class UIOverlay : MonoBehaviour
    {
        public OverlayModal OverlayModal;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var instances = FindObjectsOfType<UIOverlay>();

            if (instances.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            Global.OverlayModal = OverlayModal;
            Global.SaveManager = GetComponent<SaveManager>();
            Global.Tooltip = GetComponent<TooltipController>();
            Global.Cursor = GetComponent<CursorController>();
        }
    }
}