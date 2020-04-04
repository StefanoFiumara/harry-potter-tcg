using System;
using System.Collections;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace HarryPotter.UI
{
    public class PlayerHUDAnimationController : MonoBehaviour
    {
        public TextMeshProUGUI TurnTitle;
        public RectTransform TurnBanner;
        
        public Button BackToMainMenuBtn;
        
        private GameViewSystem _gameView;
        
        private static readonly Vector2 LeftPivot   = new Vector2(0f, 0.5f);
        private static readonly Vector2 RightPivot  = new Vector2(1f, 0.5f);
        private static readonly Vector2 MiddlePivot = new Vector2(0.5f, 0.5f);
        
        private void Awake()
        {
            if (TurnTitle == null)
            {
                Debug.LogError("PlayerHUDAnimationController: TurnTitle not set.");
                return;
            }

            if (BackToMainMenuBtn == null)
            {
                Debug.LogError("PlayerHUDAnimationController: BackToMainMenuBtn not set.");
                return;
            }
            
            _gameView = GetComponentInParent<GameViewSystem>();
            
            Global.Events.Subscribe(Notification.Prepare<ChangeTurnAction>(), SetChangeTurnAnimation);
            Global.Events.Subscribe(VictorySystem.GAME_OVER_NOTIFICATION, ShowGameOver);
        }

        private void Start()
        {
            TurnTitle.alpha = 0f;
            TurnBanner.localScale = new Vector3(0f, 1f, 1f);

            BackToMainMenuBtn.interactable = false;
            var buttonImage = BackToMainMenuBtn.GetComponent<Image>();
            var buttonText = BackToMainMenuBtn.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonImage.color = buttonImage.color.WithAlpha(0f);
            buttonText.alpha = 0f;
        }

        private void SetChangeTurnAnimation(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            
            TurnTitle.text = action.NextPlayerIndex == GameState.LOCAL_PLAYER_INDEX 
                ? "Your Turn" 
                : "Enemy's Turn";
            
            action.PerformPhase.Viewer = ChangeTurnAnimation;

        }

        private IEnumerator ChangeTurnAnimation(IContainer container, GameAction action)
        {
            var turnTextFade = DOTween.Sequence()
                .Append(TurnTitle.DOFade(1f, 0.4f))
                .AppendInterval(0.4f)
                .Append(TurnTitle.DOFade(0f, 0.4f));

            var bannerSlide = DOTween.Sequence()
                .Append(TurnBanner.DOScaleX(1f, 0.4f))
                .Append(turnTextFade)
                .AppendCallback(() => TurnBanner.SetPivot(RightPivot))
                .Append(TurnBanner.DOScaleX(0f, 0.4f))
                .AppendCallback(() => TurnBanner.SetPivot(LeftPivot));
            
            while (bannerSlide.IsPlaying())
            {
                yield return null;
            }
        }

        private void ShowGameOver(object sender, object args)
        {
            TurnTitle.text = _gameView.Game.LocalPlayer.Deck.Count == 0
                ? "You Lose"
                : "You Win!";

            DOTween.Sequence()
                .AppendCallback(() => TurnBanner.SetPivot(MiddlePivot))
                .Append(TurnBanner.DOScaleX(1f, 0.4f))
                .Join(TurnTitle.DOFade(1f, 0.4f).SetEase(Ease.Flash))
                .AppendCallback(() => TurnBanner.SetPivot(LeftPivot))
                ;

            //TODO: Might change when we implement a fancier looking button
            var buttonImage = BackToMainMenuBtn.GetComponent<Image>();
            var buttonText = BackToMainMenuBtn.GetComponentInChildren<TextMeshProUGUI>();

            DOTween.Sequence()
                .Append(buttonImage.DOFade(0.6f, 0.5f))
                .Join(buttonText.DOFade(1f, 0.5f))
                .AppendCallback(() => BackToMainMenuBtn.interactable = true);
        }

        private void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<ChangeTurnAction>(), SetChangeTurnAnimation);
            Global.Events.Unsubscribe(VictorySystem.GAME_OVER_NOTIFICATION, ShowGameOver);
        }
    }
}
