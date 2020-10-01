using System.Collections;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace HarryPotter.Input.InputStates
{
    public class PreviewState : BaseControllerState, IClickableHandler
    {
        private static readonly Vector3 ShowPreviewPosition = new Vector3
        {
            x = 0f,
            y = -3.3f,
            z = 39f
        };

        private static readonly Vector3 FaceUpRotation = new Vector3
        {
            x = 0f,
            y = 180f,
            z = 0f
        };
        
        public override void Enter()
        {
            Controller.StartCoroutine(EnterPreviewAnimation());
        }

        private IEnumerator EnterPreviewAnimation()
        {
            //TODO: Can this logic be reused anywhere else?
            var cardView = Controller.ActiveCard;

            var sequence = DOTween.Sequence()
                .Append(cardView.Move(ShowPreviewPosition, GetPreviewRotation(cardView.Card.Data.Type)));
            
            while(sequence.IsPlaying())
                yield return null;
            
            Controller.IsCardPreview = true;
        }

        private Vector3 GetPreviewRotation(CardType cardType)
        {
            var rotation = FaceUpRotation;
            
            if (cardType.IsHorizontal())
            {
                rotation.z = 90f;
            }

            return rotation;
        }
        
        public void OnClickNotification(object sender, object args)
        {
            var clickable = (Clickable) sender;
            var clickData = (PointerEventData) args;
            
            var cardView = clickable.GetComponent<CardView>();

            if (cardView == Controller.ActiveCard && clickData.button == PointerEventData.InputButton.Right)
            {
                Controller.StartCoroutine(ExitPreviewAnimation(Controller.ActiveCard));                
            }
        }

        private IEnumerator ExitPreviewAnimation(CardView cardView)
        {
            var zoneView = cardView.GetComponentInParent<ZoneView>();

            var animation = zoneView.DoZoneLayoutAnimation();

            while (animation.IsPlaying())
            {
                yield return null;
            }
            
            Controller.IsCardPreview = false;
            Controller.StateMachine.ChangeState<ResetState>();
        }
    }
}