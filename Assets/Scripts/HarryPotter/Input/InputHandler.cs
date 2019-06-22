using System.Collections.Generic;
using HarryPotter.Game;
using HarryPotter.Game.Cards;
using UnityEngine;

namespace HarryPotter.Input
{
    public enum InputType
    {
        Normal,
        Targeting
    }

    public enum TargetingType
    {
        Hand,
        Effect
    }

    [RequireComponent(typeof(GameView))]
    public class InputHandler : MonoBehaviour
    {
        private Camera _camera;
        private GameView _game;

        private InputType _inputType;
        
        private Dictionary<InputType, InputState> _stateMachine;

        public List<CardView> Targets;

        public CardView TargetSource { get; set; }
        public TargetingType TargetingType { get; set; }

        private void Awake()
        {
            _camera = Camera.main;
            _game = GetComponent<GameView>();
            _inputType = InputType.Normal;

            Targets = new List<CardView>();

            _stateMachine = new Dictionary<InputType, InputState>
            {
                {InputType.Normal, new NormalInputState(this, _game) },
                {InputType.Targeting, new TargetingInputState(this, _game) },
            };
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                
                if (Physics.Raycast(ray, out var hitInfo))
                {
                    _inputType = _stateMachine[_inputType].HandleInput(hitInfo);
                }
            }
            //TODO: Set up hover animations and descriptions
        }

        public void Select(CardView card)
        {
            Targets.Add(card);
            card.Highlight(Color.red);
        }

        public void Deselect(CardView card)
        {
            Targets.Remove(card);
            // TODO: Remove Highlight
            card.RemoveHighlight();
        }
    }
}