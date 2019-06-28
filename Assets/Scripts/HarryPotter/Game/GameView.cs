using System.Collections.Generic;
using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Player;
using UnityEngine;
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
            DOTween.Sequence().SetDelay(1f)
                .Append( DrawCards(LocalPlayer, 7) )
                .Join(   DrawCards(RemotePlayer, 7) );
        }

        //TODO: Does this method belong here?
        private Sequence DrawCards(PlayerView p, int amount = 1)
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < amount; i++)
            {
                var card = p.Zones[Zone.Deck].Cards.TakeTopCard();

                sequence.Append(p.MoveToZone(card, Zone.Hand));
            }

            return sequence;
        }

        public bool IsCardPlayable(CardView card)
        {
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

            ActionStack.Add(new GameAction(ActionType.FromHand, card, targets));
            
            return ActionStack.Resolve();
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

            ActionStack.Add(new GameAction(ActionType.InPlayEffect, card, targets));

            return ActionStack.Resolve();
        }
    }
}