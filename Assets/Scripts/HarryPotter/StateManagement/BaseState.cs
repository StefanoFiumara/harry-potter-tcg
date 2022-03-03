using HarryPotter.Systems.Core;

namespace HarryPotter.StateManagement
{
    public interface IState
    {
        IContainer Game { get; set; }
        StateMachine Owner { get; set; }

        void Enter();
        bool CanTransition(IState other);
        void Exit();
    }

    public abstract class BaseState : IState
    {
        public IContainer Game { get; set; }
        public StateMachine Owner { get; set; }

        public virtual void Enter() { }
        public virtual bool CanTransition(IState other) => true;
        public virtual void Exit() { }
    }
}
