using System.Collections.Generic;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class CardSystem : GameSystem , IAwake//, IDestroy
    {
        private TargetSystem _targetSystem;
        private MatchData _match;

        public List<Card> PlayableCards { get; set; } = new List<Card>();
        public List<Card> ActivatableCards { get; set; } = new List<Card>();
        public List<Card> SolvableAdventureCards { get; set; } = new List<Card>();

        public void Awake()
        {
            _targetSystem = Container.GetSystem<TargetSystem>();
            _match = Container.GetMatch();
        }

        public void Refresh(ControlMode mode)
        {
            PlayableCards.Clear();
            ActivatableCards.Clear();
            SolvableAdventureCards.Clear();

            foreach (var card in _match.CurrentPlayer[Zones.Hand])
            {
                _targetSystem.AutoTarget(card, AbilityType.PlayEffect, mode);
                _targetSystem.AutoTarget(card, AbilityType.PlayCondition, mode);

                var playAction = new PlayCardAction(card);
                if (playAction.Validate().IsValid)
                {
                    PlayableCards.Add(card);
                }
            }

            foreach (var card in _match.CurrentPlayer.CardsInPlay)
            {
                _targetSystem.AutoTarget(card, AbilityType.ActivateCondition, mode);
                _targetSystem.AutoTarget(card, AbilityType.ActivateEffect, mode);

                var activateAction = new ActivateCardAction(card);
                if (activateAction.Validate().IsValid)
                {
                    ActivatableCards.Add(card);
                }
            }

            foreach (var card in _match.OppositePlayer.Adventures)
            {
                _targetSystem.AutoTarget(card, AbilityType.AdventureSolveCondition, mode);
                _targetSystem.AutoTarget(card, AbilityType.AdventureSolveEffect, mode);

                var solveAction = new SolveAdventureAction(card);
                if (solveAction.Validate().IsValid)
                {
                    SolvableAdventureCards.Add(card);
                }
            }
        }

        public bool IsPlayable(Card card) => PlayableCards.Contains(card);
        public bool IsActivatable(Card card) => ActivatableCards.Contains(card);
        public bool IsSolvable(Card card) => SolvableAdventureCards.Contains(card);
        public bool IsActionable(Card card) => IsPlayable(card) || IsActivatable(card) || IsSolvable(card);
    }
}
