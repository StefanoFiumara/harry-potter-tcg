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

        public Stack<PlayAction> ActionStack;

        private void Awake()
        {
            ActionStack = new Stack<PlayAction>();
        }

        private void Start()
        {
            DOTween.Sequence()
                .Append( DrawInitialHand(LocalPlayer) )
                .Append( DrawInitialHand(RemotePlayer) );

            //TODO: Set up the ActionStack
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
    }
}