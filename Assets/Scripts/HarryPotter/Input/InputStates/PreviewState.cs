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
            Owner.StartCoroutine(EnterPreviewAnimation());
        }

        private IEnumerator EnterPreviewAnimation()
        {
            //TODO: Can this logic be reused anywhere else?
            var cardView = Owner.ActiveCard;

            var sequence = DOTween.Sequence()
                .Append(cardView.Move(ShowPreviewPosition, GetPreviewRotation(cardView.Card.Data.Type)));
            
            while(sequence.IsPlaying())
                yield return null;
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
            Owner.StartCoroutine(ExitPreviewAnimation(Owner.ActiveCard));
        }

        private IEnumerator ExitPreviewAnimation(CardView cardView)
        {
            var zoneView = cardView.GetComponentInParent<ZoneView>();

            var animation = zoneView.DoZoneLayoutAnimation();

            while (animation.IsPlaying())
            {
                yield return null;
            }
            
            Owner.StateMachine.ChangeState<ResetState>();
        }
    }
}