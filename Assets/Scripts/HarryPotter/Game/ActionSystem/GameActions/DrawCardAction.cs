using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game.Player;
using Utils;

namespace HarryPotter.Game.ActionSystem.GameActions
{
    public class DrawCardAction : IGameAction
    {
        private readonly PlayerView _player;
        private readonly int _amount;

        public DrawCardAction(PlayerView player, int amount = 1)
        {
            _player = player;
            _amount = amount;
        }

        public Sequence Execute()
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < _amount; i++)
            {
                var card = _player.Zones[Zone.Deck].Cards.TakeTopCard();

                sequence.Append(_player.MoveToZone(card, Zone.Hand));
            }

            return sequence;
        }
    }
}