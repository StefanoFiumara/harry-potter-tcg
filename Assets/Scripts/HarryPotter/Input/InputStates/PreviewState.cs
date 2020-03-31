using System.Collections;
using DG.Tweening;
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
            Owner.StartCoroutine(ShowPreviewAnimation());
        }

        private IEnumerator ShowPreviewAnimation()
        {
            //TODO: Can this logic be reused anywhere else?
            var cardView = Owner.ActiveCard;

            var sequence = DOTween.Sequence()
                .Append(cardView.transform.Move(ShowPreviewPosition, FaceUpRotation));
            
            while(sequence.IsPlaying())
                yield return null;
        }

        public void OnClickNotification(object sender, object args)
        {
            var cardView = ((Clickable) sender).GetComponent<CardView>();
            if (Owner.ActiveCard == cardView)
            {
                Owner.StartCoroutine(ReturnToHandAnimation(cardView));
            }
        }

        private IEnumerator ReturnToHandAnimation(CardView cardView)
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