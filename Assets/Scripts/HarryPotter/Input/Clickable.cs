using UnityEngine;
using UnityEngine.EventSystems;

namespace HarryPotter.Input
{
    [RequireComponent(typeof(BoxCollider))]
    public class Clickable : MonoBehaviour, IPointerClickHandler
    {
        public const string CLICKED_NOTIFICATION = "Clickable.ClickedNotification";
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Clicked {transform.name}");
            Global.Events.Publish(CLICKED_NOTIFICATION, eventData);
        }
    }
}