using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class CancelableTargetingState : TargetingState
    {
        public override void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var cardView = clickable.GetComponent<CardView>();

            var clickData = (PointerEventData) args;

            if (clickData.button == PointerEventData.InputButton.Right)
            {
                if (cardView == InputController.ActiveCard)
                {
                    CancelTargeting();
                }
                return;
            }

            if (cardView == InputController.ActiveCard)
            {
                if (Targets.Count < TargetSelector.RequiredAmount)
                {
                    CancelTargeting();
                    return;
                }
            }

            base.OnClickNotification(sender, args);
        }

        private void CancelTargeting()
        {
            Targets.Clear();

            InputController.ActiveCard.Highlight(Color.clear);
            CandidateViews.Highlight(Color.clear);

            if (ZoneInPreview != null)
            {
                ZoneInPreview.GetZoneLayoutSequence();
                ZoneInPreview = null;
            }

            Owner.ChangeState<ResetState>();
        }

        public override string GetActionText(MonoBehaviour context = null)
        {
            if (context != null && context is CardView cardView)
            {
                if (CandidateViews.Contains(cardView))
                {
                    return Targets.Contains(cardView)
                        ? $"{TextIcons.MOUSE_LEFT} Cancel Target"
                        : $"{TextIcons.MOUSE_LEFT} Target";
                }

                if (InputController.ActiveCard == cardView)
                {
                    return Targets.Count >= TargetSelector.RequiredAmount
                        ? $"{TextIcons.MOUSE_LEFT} Activate - {TextIcons.MOUSE_RIGHT} Cancel"
                        : $"{TextIcons.MOUSE_LEFT}/{TextIcons.MOUSE_RIGHT} Cancel";
                }
            }

            return string.Empty;
        }

        public override string GetDescriptionText() => string.Empty;
    }
}
