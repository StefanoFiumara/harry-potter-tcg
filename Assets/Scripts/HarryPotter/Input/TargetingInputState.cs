using System.Collections.Generic;
using HarryPotter.Game;
using HarryPotter.Game.Cards;
using HarryPotter.Game.Cards.CardAttributes;
using UnityEngine;
using Utils;

namespace HarryPotter.Input
{
    public enum TargetingType
    {
        Hand,
        Effect
    }

    public class TargetingInputState : InputState
    {
        private readonly CardView _targetingSource;
        private readonly TargetingType _targetingType;

        private readonly List<CardView> _targets;

        public TargetingInputState(InputHandler inputHandler, GameView gameView, CardView targetingSource, TargetingType targetingType) 
            : base(inputHandler, gameView)
        {
            _targetingSource = targetingSource;
            _targetingType = targetingType;
            _targets = new List<CardView>();
        }

        public override IState HandleInput(RaycastHit selection)
        {
            var card = selection.transform.GetComponent<CardView>();
            if (card != null)
            {
                if (card == _targetingSource)
                {
                    return HandleOriginalSelected(card);
                }

                if (CanBeTargeted(card))
                {
                    ToggleSelection(card);
                }
            }

            //TODO: Handle Targeting Enemy Player Deck
            return this;
        }

        private void ToggleSelection(CardView card)
        {
            if (_targets.Contains(card))
            {
                _targets.Remove(card);
                card.RemoveHighlight();
            }
            else
            {
                _targets.Add(card);
                card.Highlight(Color.red);
            }
        }

        private bool CanBeTargeted(CardView card)
        {
            var requirement = _targetingSource.GetTargetRequirement(_targetingType);

            if (_targets.Count >= requirement.NumberOfTargets) return false;

            var isMine = card.Owner == _targetingSource.Owner;

            if (requirement.TargetableTypes.Contains(card.Data.Type))
            {
                if (requirement.CanTargetEnemy && !isMine || requirement.CanTargetOwn && isMine)
                {
                    return true;
                }
            }

            return false;
        }

        // TODO: Revisit
        private IState HandleOriginalSelected(CardView card)
        {
            if (_targetingType == TargetingType.Hand)
            {
                var req = card.GetCardAttribute<FromHandTargetRequirement>();

                if (req.HasEnoughTargets(_targets.Count))
                {
                    GameView.PlayCard(card, _targets);
                }
            }
            else if (_targetingType == TargetingType.Effect)
            {
                var req = card.GetCardAttribute<FromPlayTargetRequirement>();

                if (req.HasEnoughTargets(_targets.Count))
                {
                    GameView.ActivateCard(card, _targets);
                }
            }

            return new NormalInputState(InputHandler, GameView);
        }
    }
}