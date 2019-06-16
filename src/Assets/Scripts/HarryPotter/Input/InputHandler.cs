using DG.Tweening;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Input
{
    [RequireComponent(typeof(PlayerView))]
    public class InputHandler : MonoBehaviour
    {
        private PlayerView _player;
        private Camera _camera;

        // TODO: State machine for Input Mode ?? (Selecting targets, playing from hand, etc.)

        private void Awake()
        {
            DOTween.Init().SetCapacity(500, 500);
            _player = GetComponent<PlayerView>();
            _camera = Camera.main;
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
                        _player.MoveToZone(card, Zone.Hand); // TEMP
                    }
                }
            }
        }
    }
}