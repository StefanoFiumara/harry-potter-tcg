using DG.Tweening;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Utils
{
    public static class Extensions
    {
        public static Sequence Move(this CardView cardView, Vector3 position, Vector3 rotation, float duration = 0.5f,
            float startDelay = 0f, float endDelay = 0f)
            => DOTween.Sequence()
                .AppendInterval(startDelay)
                .Append(cardView.transform.DOMove(position, duration))
                .Join(cardView.transform.DORotate(rotation, duration))
                .AppendInterval(endDelay)
                ;
        
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
    }
}