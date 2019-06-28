using System.Collections.Generic;
using HarryPotter.Game;
using HarryPotter.Game.Cards;
using UnityEngine;

namespace HarryPotter.Input
{
    [RequireComponent(typeof(GameView))]
    public class InputHandler : MonoBehaviour
    {
        private Camera _camera;
        private GameView _game;

        private IState _inputState;

        private void Awake()
        {
            _camera = Camera.main;
            _game = GetComponent<GameView>();

            _inputState = new NormalInputState(this, _game);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                
                if (Physics.Raycast(ray, out var hitInfo))
                {
                    _inputState = _inputState.HandleInput(hitInfo);
                }
            }
            //TODO: Set up hover animations and descriptions
        }
    }
}