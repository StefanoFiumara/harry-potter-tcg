using HarryPotter.Game;
using UnityEngine;

namespace HarryPotter.Input
{
    public abstract class InputState
    {
        protected readonly InputHandler InputHandler;
        protected readonly GameView GameView;

        protected InputState(InputHandler inputHandler, GameView gameView)
        {
            InputHandler = inputHandler;
            GameView = gameView;
        }

        public abstract InputType HandleInput(RaycastHit selection);
    }
}