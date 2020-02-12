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
    public class TurnBanner : MonoBehaviour
    {
        public Image Banner;
        public TextMeshProUGUI Title;

        private void Awake()
        {
            if (Banner == null || Title == null)
            {
                throw new UnityException("TurnBanner properties not set.");
            }
            
            Global.Events.Subscribe(Notification.Prepare<ChangeTurnAction>(), SetChangeTurnAnimation);
        }

        private void Start()
        {
            Banner.color = Color.clear;
            Title.alpha = 0f;
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
                .Append(Banner.DOFade(0.8f, 0.5f))
                .Append(Title.DOFade(1f, 0.5f))
                .AppendInterval(0.4f)
                .Append(Title.DOFade(0f, 0.5f))
                .Append(Banner.DOFade(0f, 0.5f));


            while (bannerSequence.IsPlaying())
            {
                yield return null;
            }

        }
    }
}
