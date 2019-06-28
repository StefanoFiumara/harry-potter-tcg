using HarryPotter.Game;
using UnityEngine;

namespace HarryPotter.Input
{
    public interface IState
    {
        IState HandleInput(RaycastHit selection);
    }

    public abstract class InputState : IState
    {
        protected readonly InputHandler InputHandler;
        protected readonly GameView GameView;

        protected InputState(InputHandler inputHandler, GameView gameView)
        {
            InputHandler = inputHandler;
            GameView = gameView;
        }

        public abstract IState HandleInput(RaycastHit selection);
    }
}