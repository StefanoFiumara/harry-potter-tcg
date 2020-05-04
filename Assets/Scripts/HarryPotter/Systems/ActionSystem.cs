using System.Collections;
using System.Collections.Generic;
using HarryPotter.GameActions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class ActionSystem : GameSystem, IAwake
    {
        public const string BEGIN_SEQUENCE_NOTIFICATION = "ActionSystem.beginSequenceNotification";
        public const string END_SEQUENCE_NOTIFICATION = "ActionSystem.endSequenceNotification";
        public const string DEATH_REAPER_NOTIFICATION = "ActionSystem.deathReaperNotification";
        public const string COMPLETE_NOTIFICATION = "ActionSystem.completeNotification";

        public bool IsActive => _rootSequence != null;

        private GameAction _rootAction;
        private IEnumerator _rootSequence;
        private List<GameAction> _openReactions;
        private VictorySystem _victorySystem;

        public void Awake()
        {
            _victorySystem = Container.GetSystem<VictorySystem>();
        }

        public void Perform(GameAction action)
        {
            if (IsActive)
            {
                Debug.LogWarning("Attempted to perform while sequence in progress.");
                return;
            }

            _rootAction = action;
            _rootSequence = Sequence(action);
            Debug.Log($"Action: {action}");
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
            if (_openReactions == null)
            {
                Debug.LogWarning("Attempted to add a reaction at the wrong time.");
            }
            Debug.Log($"    -> Reaction: {action}");
            _openReactions?.Add(action);
        }

        private IEnumerator Sequence(GameAction action)
        {
            Global.Events.Publish(BEGIN_SEQUENCE_NOTIFICATION, action);

            if (action.Validate() == false || _victorySystem.IsGameOver())
            {
                action.Cancel();
            }
            
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

            phase = MainPhase(action.CancelPhase);
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
            if (phase.Owner.IsCanceled ^ phase == phase.Owner.CancelPhase)
            {
                yield break;
            }
            
            var reactions = _openReactions = new List<GameAction>();
            
            var flow = phase.Flow (Container);
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
                var subFlow = Sequence(reaction);
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
    
    public static class ActionSystemExtensions {
        public static void Perform (this IContainer game, GameAction action) {
            //TODO: Maybe here is where we send actions to remote players via RPC calls
            //TODO: Network System as part of game container?
            var actionSystem = game.GetSystem<ActionSystem>();
            actionSystem.Perform(action);
        }

        public static void AddReaction (this IContainer game, GameAction action) {
            var actionSystem = game.GetSystem<ActionSystem>();
            actionSystem.AddReaction(action);
        }
    }
}