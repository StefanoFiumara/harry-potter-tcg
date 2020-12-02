using System.Collections;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.Views
{
    public class PlayerView : MonoBehaviour
    {
        private static readonly Vector2 LeftPivot   = new Vector2(0f, 0.5f);
        private static readonly Vector2 RightPivot  = new Vector2(1f, 0.5f);
        private static readonly Vector2 MiddlePivot = new Vector2(0.5f, 0.5f);
        
        private IContainer _gameContainer;
        private MatchData _match;
        private GameViewSystem _gameView;

        public TextMeshProUGUI TurnTitle;
        public RectTransform TurnBanner;
        
        public Button EndTurnBtn;
        public Button DrawCardBtn;
        public Button BackToMainMenuBtn;

        private bool IsActive => 
            _match.CurrentPlayer.ControlMode == ControlMode.Local 
            && _gameView.IsIdle;
        
        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _gameContainer = _gameView.Container;
            _match = _gameContainer.Match;
            
            Global.Events.Subscribe(Notification.Prepare<ChangeTurnAction>(), OnPrepareChangeTurn);
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
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
        
        private void OnPrepareChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;

            if (action.NextPlayerIndex == _match.EnemyPlayer.Index)
            {
                EndTurnBtn.interactable = false;
                DrawCardBtn.interactable = false;
                _gameView.Cursor.ResetCursor();
            }
            
            TurnTitle.text = action.NextPlayerIndex == _match.LocalPlayer.Index 
                ? "Your Turn" 
                : "Enemy's Turn";
            
            action.PerformPhase.Viewer = ChangeTurnAnimation;
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;

            if (action.NextPlayerIndex == _match.LocalPlayer.Index)
            {
                EndTurnBtn.interactable = true;
                DrawCardBtn.interactable = true;
            }
        }
        
        private void ShowGameOver(object sender, object args)
        {
            TurnTitle.text = _gameView.Match.LocalPlayer.Deck.Count == 0
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

        // TODO: These feel out of place, should handling game events be centralized to the Input System?
        public void OnClickChangeTurn()
        {
            if (IsActive)
            {
                _gameContainer.ChangeTurn();
            }
        }

        public void OnClickDrawCard()
        {
            if (IsActive)
            {
                var handSystem = _gameContainer.GetSystem<HandSystem>();
                handSystem.DrawCards(_match.CurrentPlayer, 1, true);
            }
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

            yield return null;
            while (bannerSlide.IsPlaying())
            {
                yield return null;
            }
        }
        
        public void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<ChangeTurnAction>(), OnPrepareChangeTurn);
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
            Global.Events.Unsubscribe(VictorySystem.GAME_OVER_NOTIFICATION, ShowGameOver);
        }
    }
}