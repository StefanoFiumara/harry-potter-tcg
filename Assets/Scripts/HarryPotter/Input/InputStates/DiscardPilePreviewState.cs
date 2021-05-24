using HarryPotter.Enums;
using HarryPotter.Views.UI;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class DiscardPilePreviewState : PreviewState
    {
        public override void Enter()
        {
            ZoneInPreview = InputController.GameView.FindZoneView(InputController.ActivePlayer, Zones.Discard);
            ZoneInPreview.GetPreviewSequence(sortOrder: PreviewSortOrder.ByType);
        }

        public override void OnClickNotification(object sender, object args)
        {
            // Do nothing.
        }

        public void ExitPreview()
        {
            InputController.StartCoroutine(ExitPreviewAnimation());
        }
        
        public override string GetActionText(MonoBehaviour context = null)
        {
            return $"{TextIcons.MOUSE_LEFT} / {TextIcons.MOUSE_RIGHT} Exit Preview";
        }
    }
}