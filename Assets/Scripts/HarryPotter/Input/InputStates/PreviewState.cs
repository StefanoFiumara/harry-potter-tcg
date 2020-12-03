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
        
        public override void Enter()
        {
            var cardView = InputSystem.ActiveCard;
            var cardType = cardView.Card.Data.Type;
            
            var previewRotation = ZoneView.GetRotation(isFaceDown: false, isHorizontal: cardType.IsHorizontal(), isEnemy: false);
            cardView.Move(CardPreviewPosition, previewRotation);            
        }

        public void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var clickData = (PointerEventData) args;
            
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == InputSystem.ActiveCard && clickData.button == PointerEventData.InputButton.Right)
            {
                InputSystem.StartCoroutine(ExitPreviewAnimation(InputSystem.ActiveCard));                
            }
        }

        private IEnumerator ExitPreviewAnimation(CardView cardView)
        {
            var zoneView = InputSystem.GameView.FindZoneView(cardView.Card.Owner, cardView.Card.Zone);

            var animation = zoneView.GetZoneLayoutSequence();

            while (animation.IsPlaying())
            {
                yield return null;
            }
            
            InputSystem.StateMachine.ChangeState<ResetState>();
        }

        public string GetDescriptionText() => string.Empty;

        public string GetActionText(MonoBehaviour context = null)
        {
            if (context is CardView cardView)
            {
                if (cardView == InputSystem.ActiveCard)
                {
                    return $"{TextIcons.MOUSE_RIGHT} Back";
                }
            }

            return string.Empty;
        }
    }
}