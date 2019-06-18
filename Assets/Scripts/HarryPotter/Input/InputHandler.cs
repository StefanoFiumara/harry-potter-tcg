using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Input
{    
    [RequireComponent(typeof(GameView))]
    public class InputHandler : MonoBehaviour
    {
        private Camera _camera;
        private GameView _game;

        // TODO: State machine for Input Mode ?? (Selecting targets, playing from hand, etc.)

        private void Awake()
        {
            _camera = Camera.main;
            _game = GetComponent<GameView>();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                
                if (Physics.Raycast(ray, out var hitInfo))
                {
                    var card = hitInfo.transform.GetComponent<CardView>();
                    
                    if (card != null)
                    {
                        //TODO: Use GameView to trigger an Action on the stack
                        card.Owner.MoveToZone(card, Zone.Hand); // TEMP
                    }
                }
            }

            //TODO: Set up hover animations and descriptions
        }
    }
}