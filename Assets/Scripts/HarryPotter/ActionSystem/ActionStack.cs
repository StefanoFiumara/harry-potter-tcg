using System.Collections;
using System.Collections.Generic;
using HarryPotter.Data;

namespace HarryPotter.ActionSystem
{
    public class ActionStack
    {
        public const string BEGIN_SEQUENCE_NOTIFICATION = "ActionStack.beginSequenceNotification";
        public const string END_SEQUENCE_NOTIFICATION = "ActionStack.endSequenceNotification";
        public const string DEATH_REAPER_NOTIFICATION = "ActionStack.deathReaperNotification";
        public const string COMPLETE_NOTIFICATION = "ActionStack.completeNotification";

        public bool IsActive => _rootSequence != null;

        private readonly GameState _gameState;
        private GameAction _rootAction;
        private IEnumerator _rootSequence;
        private List<GameAction> _openReactions;

        public ActionStack(GameState gameState)
        {
            _gameState = gameState;
        }
        
        public void Perform(GameAction action)
        {
            if (IsActive) return;

            _rootAction = action;
            _rootSequence = Sequence(action);
        }

        public void Update()
        {
            if (_rootSequence == null) return;

            if (_rootSequence.MoveNext() == false)
            {
                _rootAction = null;
                _rootSequence = null;
                _openReactions = null;
                Global.Events.Publish(COMPLETE_NOTIFICATION);
            }
        }

        public void AddReaction(GameAction action)
        {
            _openReactions?.Add(action);
        }

        private IEnumerator Sequence(GameAction action)
        {
            Global.Events.Publish(BEGIN_SEQUENCE_NOTIFICATION, action);

            var phase = MainPhase(action.PreparePhase);
            while (phase.MoveNext())
            {
                yield return null;
            }

            phase = MainPhase(action.PerformPhase);
            while (phase.MoveNext())
            {
                yield return null;
            }

            if (_rootAction == action)
            {
                phase = EventPhase(DEATH_REAPER_NOTIFICATION, action, true);
                while (phase.MoveNext())
                {
                    yield return null;
                }
            }
            
            Global.Events.Publish(END_SEQUENCE_NOTIFICATION, action);
        }
        
        private IEnumerator MainPhase (Phase phase) 
        {
            if (phase.Owner.IsCanceled) yield break;
            
            var reactions = _openReactions = new List<GameAction>();
            
            var flow = phase.Flow (_gameState);
            while (flow.MoveNext())
            {
                yield return null;
            }
            
            flow = ReactPhase (reactions);
            while (flow.MoveNext())
            {
                yield return null;
            }
        }

        private IEnumerator ReactPhase(List<GameAction> reactions)
        {
            reactions.Sort(SortActions);
            foreach (var reaction in reactions)
            {
                IEnumerator subFlow = Sequence(reaction);
                while (subFlow.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator EventPhase(string notification, GameAction action, bool repeats = false)
        {
            List<GameAction> reactions;
            do
            {
                reactions = _openReactions = new List<GameAction>();
                Global.Events.Publish(notification, action);
                var phase = ReactPhase(reactions);
                while (phase.MoveNext())
                {
                    yield return null;
                }
            } while (repeats && reactions.Count > 0);
        }

        int SortActions(GameAction x, GameAction y)
        {
            if (x.Priority != y.Priority)
            {
                return y.Priority.CompareTo(x.Priority);
            }

            return x.OrderOfPlay.CompareTo(y.OrderOfPlay);
        }

    }
}