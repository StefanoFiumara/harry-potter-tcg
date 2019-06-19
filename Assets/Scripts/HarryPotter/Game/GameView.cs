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
                .Append( DrawInitialHand(LocalPlayer) )
                .Join( DrawInitialHand(RemotePlayer) );
        }

        //TODO: Does this method belong here?
        private Sequence DrawInitialHand(PlayerView p)
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < 7; i++)
            {
                var card = p.Zones[Zone.Deck].Cards.TakeTopCard();

                sequence.Append(p.MoveToZone(card, Zone.Hand));
            }

            return sequence;
        }

        // TODO: Will need to be adjusted if we decide to get fancy with the outline/highlight system later
        public bool IsCardPlayable(CardView card)
        {
            if (card.Owner != LocalPlayer) return false;
            if (!ActionStack.IsEmpty) return false;

            if (card.State.Zone == Zone.Hand)
            {
                return card.Data.PlayConditions.MeetsConditions(GameState);
            }

            if (card.State.Zone.IsInPlay())
            {
                return card.Data.ActivateConditions.MeetsConditions(GameState);
            }

            return false;
        }

        public Sequence PlayCard(CardView card)
        {
            //TODO: Check if card requires targets
            var owner = card.Owner;
            var enemy = owner == LocalPlayer ? RemotePlayer : LocalPlayer;

            foreach (var condition in card.Data.PlayConditions)
            {
                condition.Satisfy(owner.PlayerState, enemy.PlayerState);
            }

            ActionStack.Add(new GameAction(ActionType.FromHand, card));
            
            return ActionStack.Resolve();
        }
    }
}