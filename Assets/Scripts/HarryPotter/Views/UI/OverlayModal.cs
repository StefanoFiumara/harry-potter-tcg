using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.Views.UI
{
    public class OverlayModal : MonoBehaviour
    {
        public TMP_Text Title;
        public TMP_Text Message;

        public Button OkButton;
        
        public TMP_Text OkLabel;
        public TMP_Text CancelLabel;
        
        private Action _okCallback;
        private Action _exitCallback;
        
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        // TODO: Parameters for OK/Cancel label? enum for multiple button settings?
        public void ShowModal(string title, string message, Action okCallback = null, Action exitCallback = null)
        {
            Title.text = title;
            Message.text = message;

            if (okCallback != null)
            {
                _okCallback = okCallback;
                _exitCallback = exitCallback;
                OkLabel.text = "OK";
                CancelLabel.text = "CANCEL";
                OkButton.gameObject.SetActive(true);
            }
            else
            {
                CancelLabel.text = "OK";
                OkButton.gameObject.SetActive(false);
            }
            
            gameObject.SetActive(true);
        }

        public void OnExitButtonClicked()
        {
            gameObject.SetActive(false);
            _exitCallback?.Invoke();
        }

        public void OnOkButtonClicked()
        {
            gameObject.SetActive(false);
            _okCallback?.Invoke();
        }
    }
}