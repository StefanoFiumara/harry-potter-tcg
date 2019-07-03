using System.Collections.Generic;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game.ActionSystem.GameActions;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Player;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Utils;

namespace HarryPotter.Game
{
    public class GameView : MonoBehaviour
    {
        public GameState GameState;

        public PlayerView LocalPlayer;
        public PlayerView RemotePlayer;

        public ActionStack ActionStack;

        private void Awake()
        {
            DOTween.Init().SetCapacity(4000, 1500);

            ActionStack = new ActionStack(GameState);
        }

        private void Start()
        {
            ActionStack.Add(new DrawCardAction(RemotePlayer, 7));
            ActionStack.Add(new DrawCardAction(LocalPlayer, 7));

            ActionStack.Resolve()
                .OnComplete(() =>
                {
                    LocalPlayer.PlayerState.ActionsAvailable = 2;
                });
        }

        public bool IsCardPlayable(CardView card)
        {
            // TODO: This logic will have to change for multiplayer...
            if (card.Owner != LocalPlayer) return false;
            if (!ActionStack.IsEmpty) return false;

            if (card.State.Zone == Zone.Hand)
            {
                return card.Data.PlayConditions.MeetsConditions(GameState);
            }

            return false;
        }

        public bool IsCardActivatable(CardView card)
        {
            if (card.Owner != LocalPlayer) return false;
            if (!ActionStack.IsEmpty) return false;

            if (card.State.Zone.IsInPlay())
            {
                return card.Data.ActivateConditions.MeetsConditions(GameState);
            }

            return false;
        }

        public Sequence PlayCard(CardView card, List<CardView> targets = null)
        {
            var owner = card.Owner;
            var enemy = owner == LocalPlayer ? RemotePlayer : LocalPlayer;

            targets = targets ?? new List<CardView>();

            foreach (var condition in card.Data.PlayConditions)
            {
                condition.Satisfy(owner.PlayerState, enemy.PlayerState);
            }

            ActionStack.Add(new PlayCardFromHandAction(card, targets));
            
            return ActionStack.Resolve();
        }

        public void OnClickDrawCard()
        {
            if (!ActionStack.IsEmpty) return;
            if (LocalPlayer.PlayerState.ActionsAvailable > 0)
            {
                ActionStack.Add(new DrawCardAction(LocalPlayer));

                ActionStack.Resolve()
                    .OnComplete(() =>
                {
                    LocalPlayer.PlayerState.ActionsAvailable--;
                });
            }
        }

        public Sequence ActivateCard(CardView card, List<CardView> targets = null)
        {
            var owner = card.Owner;
            var enemy = owner == LocalPlayer ? RemotePlayer : LocalPlayer;

            targets = targets ?? new List<CardView>();

            foreach (var condition in card.Data.ActivateConditions)
            {
                condition.Satisfy(owner.PlayerState, enemy.PlayerState);
            }

            ActionStack.Add(new ActivateInPlayCardEffectAction(card, targets));

            return ActionStack.Resolve();
        }
    }
}