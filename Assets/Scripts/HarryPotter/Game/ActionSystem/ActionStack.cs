using System.Collections.Generic;
using DG.Tweening;

namespace HarryPotter.Game
{
    public class ActionStack
    {
        private readonly GameState _gameState;

        private readonly List<GameAction> _stack;

        public bool IsEmpty => _stack.Count == 0;

        public ActionStack(GameState gameState)
        {
            _gameState = gameState;
            _stack = new List<GameAction>();
        }

        public void Add(GameAction action)
        {
            //var inPlayZones = new[] { Zone.Lessons, Zone.Creatures, Zone.Characters, Zone.Items, Zone.Match, Zone.Location, Zone.Adventure };
            //var cardsInPlay = _gameState.LocalPlayer.Cards.Where(c => inPlayZones.Contains(c.Zone));

            //TODO: Announce card being played and gather reactions
            //TODO: Add any reactions to the stack
            var reactions = new List<GameAction>(); //TODO: Figure out how to gather reactions

            _stack.Add(action);

            foreach (var reaction in reactions)
            {
                Add(reaction);
            }
        }

        public Sequence Resolve()
        {
            var sequence = DOTween.Sequence();

            var nextIndex = _stack.Count;

            while (nextIndex > 0)
            {
                nextIndex--;
                var next = _stack[nextIndex];

                foreach (var action in next.CardActions)
                {
                    sequence.Append(action.Execute(next.Card, next.Targets)
                            .AppendCallback(() => _stack.Remove(next)));
                }
            }

            return sequence;
        }
    }
}