using System;
using System.Collections;
using HarryPotter.Data;

namespace HarryPotter.ActionSystem
{
    public class Phase
    {
        public GameAction Owner { get; }
        public Action<GameState> Handler { get; }

        public Func<GameState, GameAction, IEnumerator> Viewer { get; set; }

        public Phase(GameAction owner, Action<GameState> handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public IEnumerator Flow(GameState gameState)
        {
            bool hitKeyFrame = false;

            if (Viewer != null)
            {
                var sequence = Viewer(gameState, Owner);
                while (sequence.MoveNext())
                {
                    var isKeyFrame = (sequence.Current is bool) ? (bool) sequence.Current : false;

                    if (isKeyFrame)
                    {
                        hitKeyFrame = true;
                        Handler(gameState);
                    }

                    yield return null;
                }
            }

            if (!hitKeyFrame)
            {
                Handler(gameState);
            }
        }
    }
}