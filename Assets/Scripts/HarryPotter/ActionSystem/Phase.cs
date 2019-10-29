using System;
using System.Collections;
using HarryPotter.Data;

namespace HarryPotter.ActionSystem
{
    public class Phase
    {
        public GameAction Owner { get; }
        public Action<Game> Handler { get; }

        public Func<Game, GameAction, IEnumerator> Viewer { get; set; }

        public Phase(GameAction owner, Action<Game> handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public IEnumerator Flow(Game game)
        {
            bool hitKeyFrame = false;

            if (Viewer != null)
            {
                var sequence = Viewer(game, Owner);
                while (sequence.MoveNext())
                {
                    var isKeyFrame = (sequence.Current is bool) ? (bool) sequence.Current : false;

                    if (isKeyFrame)
                    {
                        hitKeyFrame = true;
                        Handler(game);
                    }

                    yield return null;
                }
            }

            if (!hitKeyFrame)
            {
                Handler(game);
            }
        }
    }
}