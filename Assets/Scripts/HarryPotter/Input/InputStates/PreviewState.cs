using System.Collections;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input.InputStates
{
    public class PreviewState : BaseInputState, IClickableHandler, ITooltipContent
    {
        private static readonly Vector3 CardPreviewPosition = new Vector3
        {
            x = 0f,
            y = -3.3f,
            z = 39f
        };

        protected ZoneView ZoneInPreview;

        public override void Enter()
        {
            var cardView = InputController.ActiveCard;
            var cardType = cardView.Card.Data.Type;
            
            var previewRotation = ZoneView.GetRotation(isFaceDown: false, isHorizontal: cardType.IsHorizontal(), isEnemy: false);

            cardView.SetSortingLayer(9999);
            cardView.Move(CardPreviewPosition, previewRotation);
            
            ZoneInPreview = InputController.GameView.FindZoneView(cardView.Card.Owner, cardView.Card.Zone);
        }

        public virtual void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var clickData = (PointerEventData) args;
            
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == InputController.ActiveCard && clickData.button == PointerEventData.InputButton.Right)
            {
                InputController.StartCoroutine(ExitPreviewAnimation());                
            }
        }

        protected IEnumerator ExitPreviewAnimation()
        {
            var animation = ZoneInPreview.GetZoneLayoutSequence();

            while (animation.IsPlaying())
            {
                yield return null;
            }
            
            InputController.StateMachine.ChangeState<ResetState>();
        }

        public string GetDescriptionText() => string.Empty;
        
        public override void Exit()
        {
            if (ZoneInPreview != null)
            {
                ZoneInPreview.GetZoneLayoutSequence();
                ZoneInPreview = null;
            }
        }

        public virtual string GetActionText(MonoBehaviour context = null)
        {
            if (context is CardView cardView)
            {
                if (cardView == InputController.ActiveCard)
                {
                    return $"{TextIcons.MOUSE_RIGHT} Back";
                }
            }

            return string.Empty;
        }
    }
}