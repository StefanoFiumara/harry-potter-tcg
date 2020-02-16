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
        public static readonly Vector3 SHOW_PREVIEW_POSITION = new Vector3
        {
            x = 0f,
            y = -3.3f,
            z = 39f
        };
        
        public static readonly Vector3 FACE_UP_ROTATION = new Vector3
        {
            x = 0f,
            y = 180f,
            z = 0f
        };


        
        public override void Enter()
        {
            Owner.StartCoroutine(ShowPreviewAnimation());
        }

        public IEnumerator ShowPreviewAnimation()
        {
            //TODO: Merge this with a show preview animation in HandView/ZoneView?
            var cardView = Owner.ActiveCard;

            var sequence = DOTween.Sequence()
                .Append(cardView.transform.Move(SHOW_PREVIEW_POSITION, FACE_UP_ROTATION));
            
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