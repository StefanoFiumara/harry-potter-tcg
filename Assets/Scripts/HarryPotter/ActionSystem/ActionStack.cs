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

        private GameAction RootAction { get; set; }
        private IEnumerator RootSequence { get; set; }
        private List<GameAction> OpenReactions { get; set; }
        public bool IsActive => RootSequence != null;

        private readonly Game _game;
        
        public ActionStack(Game game)
        {
            _game = game;
        }
        
        public void Perform(GameAction action)
        {
            if (IsActive) return;

            RootAction = action;
            RootSequence = Sequence(action);
        }

        public void Update()
        {
            if (RootSequence == null) return;

            if (RootSequence.MoveNext() == false)
            {
                RootAction = null;
                RootSequence = null;
                OpenReactions = null;
                Global.Events.Publish(COMPLETE_NOTIFICATION);
            }
        }

        public void AddReaction(GameAction action)
        {
            OpenReactions?.Add(action);
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

            if (RootAction == action)
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
            
            var reactions = OpenReactions = new List<GameAction>();
            
            var flow = phase.Flow (_game);
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
            foreach (GameAction reaction in reactions)
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
                reactions = OpenReactions = new List<GameAction>();
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