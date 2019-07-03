using System.Collections.Generic;
using DG.Tweening;
using HarryPotter.Game.Cards;

namespace HarryPotter.Game.ActionSystem.GameActions
{
    public abstract class ExecuteCardAction : IGameAction
    {
        private readonly CardView _card;
        private readonly List<CardView> _targets;
        private readonly IEnumerable<CardAction> _actions;

        protected ExecuteCardAction(CardView card, List<CardView> targets, IEnumerable<CardAction> actions)
        {
            _card = card;
            _targets = targets;
            _actions = actions;
        }

        public Sequence Execute()
        {
            var sequence = DOTween.Sequence();

            foreach (var action in _actions)
            {
                sequence.Append(action.Execute(_card, _targets));
            }

            return sequence;
        }
    }
}