using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    public class HealingView : MonoBehaviour
    {
        private static readonly Vector3 HealingPreviewPosition = new Vector3
        {
            x = -27.5f,
            y = 0f,
            z = 79f
        };
        
        private static readonly Vector3 HealingPreviewPositionEnemy = new Vector3
        {
            x = 7.95f,
            y = 0f,
            z = 79f
        };
        
        private static readonly Vector2 HealingPreviewSpacing = new Vector2
        {
            x = 1.1f,
            y = 0.2f
        };
        
        private static readonly int HealingPreviewColumnCount = 5;

        private GameView _gameView;

        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<HealingAction>(), OnPrepareHealing);
            
            _gameView = GetComponent<GameView>();

            if (_gameView == null)
            {
                Debug.LogError("HealingView could not find GameView");
            }
        }

        private void OnPrepareHealing(object sender, object args)
        {
            var action = (HealingAction) args;
            action.PerformPhase.Viewer = HealingAnimation;
        }

        private IEnumerator HealingAnimation(IContainer container, GameAction action)
        {
            var healAction = (HealingAction) action;
            yield return true;

            var cardViews = _gameView.FindCardViews(healAction.HealedCards);

            var sequence = _gameView
                .GetParticleSequence(action, Zones.Deck)
                .Append(GetHealingSequence(cardViews));
            
            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }

        public Sequence GetHealingSequence(List<CardView> targets, float duration = 0.5f)
        {
            var healSequence = DOTween.Sequence();
            var animationTime = 0f;
            
            // NOTE: Hack to move these out of the loop
            //       Can't just pass in source because the animation would be wrong in the case where a player's card heals the opponent
            var player = targets[0].Card.Owner;
            var startZoneView = _gameView.FindZoneView(player, Zones.Discard);
            var endZoneView = _gameView.FindZoneView(player, Zones.Deck);

            targets = targets
                .OrderBy(c => c.Card.Data.Type)
                .ThenBy(c => c.Card.GetLessonType())
                .ToList();
            
            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                var startPos = player == _gameView.Match.LocalPlayer ? HealingPreviewPosition : HealingPreviewPositionEnemy;
                
                var targetPos = ZoneView.GetPosition(startPos, i, HealingPreviewSpacing, HealingPreviewColumnCount);
                var targetRot = ZoneView.GetRotation(isFaceDown: false, isHorizontal: false, isEnemy: false);
                
                healSequence.Insert(animationTime, target.Move(targetPos, targetRot, duration));
                
                _gameView.ChangeZoneView(target, to: Zones.Deck, @from: Zones.Discard);
                animationTime += 0.25f;
            }
            
            
            healSequence
                    // TODO: AppendInterval here to preview the healed cards for a little longer tweening to deck?
                .Append(startZoneView.GetZoneLayoutSequence(duration))
                .Join(endZoneView.GetZoneLayoutSequence(duration));
            
            return healSequence;
        }
        
        private void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<HealingAction>(), OnPrepareHealing);
        }
    }
}