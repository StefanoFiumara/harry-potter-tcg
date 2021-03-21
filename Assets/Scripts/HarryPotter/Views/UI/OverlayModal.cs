using System;
using TMPro;
using UnityEngine;

namespace HarryPotter.Views.UI
{
    public class OverlayModal : MonoBehaviour
    {
        public TMP_Text Title;
        public TMP_Text Message;
        
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void ShowModal(string title, string message)
        {
            Title.text = title;
            Message.text = message;
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}