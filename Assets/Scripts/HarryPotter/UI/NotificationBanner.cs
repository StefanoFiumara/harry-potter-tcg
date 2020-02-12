using System;
using System.Collections;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.GameActions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace HarryPotter.UI
{
    public class NotificationBanner : MonoBehaviour
    {
        public Image Banner;
        public TextMeshProUGUI Title;
        public Button BackToMainMenuBtn;
        
        private GameViewSystem _gameView;
        
        private void Awake()
        {
            if (Banner == null || Title == null)
            {
                throw new UnityException("NotificationBanner properties not set.");
            }
            
            _gameView = GetComponentInParent<GameViewSystem>();
            
            Global.Events.Subscribe(Notification.Prepare<ChangeTurnAction>(), SetChangeTurnAnimation);
            Global.Events.Subscribe(VictorySystem.GAME_OVER_NOTIFICATION, ShowGameOver);
        }

        private void Start()
        {
            Banner.color = Color.clear;
            Title.alpha = 0f;
            
            var buttonImage = BackToMainMenuBtn.GetComponent<Image>();
            var buttonText = BackToMainMenuBtn.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonImage.color = buttonImage.color.WithAlpha(0f);
            buttonText.alpha = 0f;
        }

        private void SetChangeTurnAnimation(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            
            Title.text = action.NextPlayerIndex == GameState.LOCAL_PLAYER_INDEX 
                ? "Your Turn" 
                : "Enemy's Turn";
            
            action.PerformPhase.Viewer = ChangeTurnAnimation;

        }

        private IEnumerator ChangeTurnAnimation(IContainer container, GameAction action)
        {
            var bannerSequence = DOTween.Sequence()
                .Append(Banner.DOFade(0.8f, 0.4f))
                .Append(Title.DOFade(1f, 0.4f))
                .AppendInterval(0.4f)
                .Append(Title.DOFade(0f, 0.4f))
                .Append(Banner.DOFade(0f, 0.4f));

            while (bannerSequence.IsPlaying())
            {
                yield return null;
            }
        }

        private void ShowGameOver(object sender, object args)
        {
            
            Title.text = _gameView.Game.LocalPlayer.Deck.Count == 0
                ? "You Lose"
                : "You Win!";

            DOTween.Sequence()
                .Append(Banner.DOFade(0.8f, 0.4f))
                .Append(Title.DOFade(1f, 0.4f).SetEase(Ease.Flash));

            //TODO: Might change when we implement a fancier looking button
            var buttonImage = BackToMainMenuBtn.GetComponent<Image>();
            var buttonText = BackToMainMenuBtn.GetComponentInChildren<TextMeshProUGUI>();

            DOTween.Sequence()
                .Append(buttonImage.DOFade(1f, 0.5f))
                .Join(buttonText.DOFade(1f, 0.5f));
        }

        private void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<ChangeTurnAction>(), SetChangeTurnAnimation);
        }
    }
}
