using HarryPotter.Systems.Core;

namespace HarryPotter.StateManagement
{
    public interface IState : IGameSystem
    {
        void Enter();
        bool CanTransition(IState other);
        void Exit();
    }
    
    public abstract class BaseState : GameSystem, IState
    {
        public virtual void Enter() { }
        public virtual bool CanTransition(IState other) => true;
        public virtual void Exit() { }
    }
}