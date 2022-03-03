using System.Collections;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Input.InputStates;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;
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

        private IContainer _game;
        private MatchData _match;
        private GameView _gameView;

        public TextMeshProUGUI TurnTitle;
        public RectTransform TurnBanner;

        public Button EndTurnBtn;
        public Button DrawCardBtn;
        public Button BackToMainMenuBtn;
        public Button ForfeitBtn;

        private bool IsHudActive =>
            _match.CurrentPlayer.ControlMode == ControlMode.Local
            && _gameView.IsIdle;

        private void Awake()
        {
            _gameView = GetComponentInParent<GameView>();
            _game = _gameView.Container;
            _match = _game.GetMatch();

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
                ForfeitBtn.interactable = false;
                Global.Cursor.ResetCursor();
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
                ForfeitBtn.interactable = true;
                DrawCardBtn.interactable = true;
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

        private void ShowGameOver(object sender, object args)
        {
            var winner = _game.GetSystem<VictorySystem>().Winner;

            if (winner == null)
            {
                Debug.LogWarning("Called ShowGameOver sequence with no winner set!");
            }

            var buttonImage = BackToMainMenuBtn.GetComponent<Image>();
            var buttonText = BackToMainMenuBtn.GetComponentInChildren<TextMeshProUGUI>();

            TurnTitle.text = $"{winner.PlayerName} Wins the game!";

            BackToMainMenuBtn.interactable = true;
            ForfeitBtn.interactable = false;
            EndTurnBtn.interactable = false;
            DrawCardBtn.interactable = false;

            DOTween.Sequence()
                .AppendCallback(() => TurnBanner.SetPivot(MiddlePivot))
                .Append(TurnBanner.DOScaleX(1f, 0.4f))
                .Join(TurnTitle.DOFade(1f, 0.4f).SetEase(Ease.Flash))
                .AppendCallback(() => TurnBanner.SetPivot(LeftPivot))
                .Append(buttonImage.DOFade(0.6f, 0.5f))
                .Join(buttonText.DOFade(1f, 0.5f))
                ;
        }

        // TODO: These feel out of place, should handling game events be centralized to the Input System?

        public void OnClickChangeTurn()
        {
            if (IsHudActive && _gameView.Input.StateMachine.CurrentState is not TargetingState)
            {
                Debug.Log("*** PLAYER ENDS TURN ***");
                _game.ChangeTurn();
            }
        }

        public void OnClickDrawCard()
        {
            if (IsHudActive && _gameView.Input.StateMachine.CurrentState is not TargetingState)
            {
                var handSystem = _game.GetSystem<HandSystem>();
                handSystem.DrawCards(_match.CurrentPlayer, 1, true);
            }
        }

        public void OnClickForfeit()
        {
            if (IsHudActive)
            {
                Global.OverlayModal.ShowModal(
                    "Are you sure?",
                    "Are you sure you want to forfeit the game?",
                    okCallback: () =>
                    {
                        _game.SetWinner(_match.EnemyPlayer);
                        _game.ChangeState<GameOverState>();
                        Debug.Log($"*** PLAYER {_match.LocalPlayer.PlayerName} FORFEITED THE GAME ***");
                    });
            }
        }

        public void OnClickViewPlayerDiscardPile(TMP_Text senderLabel)
        {
            if (IsHudActive && _gameView.Input.StateMachine.CurrentState is not TargetingState)
            {
                ViewDiscardPile(_gameView.Match.LocalPlayer, senderLabel);
            }
        }

        public void OnClickViewEnemyDiscardPile(TMP_Text senderLabel)
        {
            if (IsHudActive && _gameView.Input.StateMachine.CurrentState is not TargetingState)
            {
                ViewDiscardPile(_gameView.Match.EnemyPlayer, senderLabel);
            }
        }

        public void ViewDiscardPile(Player target, TMP_Text senderLabel)
        {
            if (_gameView.Input.StateMachine.CurrentState is WaitingForInputState)
            {
                _gameView.Input.SetActivePlayer(target);
                _gameView.Input.StateMachine.ChangeState<DiscardPilePreviewState>();
                senderLabel.text = "Back";
            }
            else if (_gameView.Input.StateMachine.CurrentState is DiscardPilePreviewState previewState)
            {
                senderLabel.text = "View";
                previewState.ExitPreview();
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
