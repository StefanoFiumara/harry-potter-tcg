using HarryPotter.Systems.Core;

namespace HarryPotter.StateManagement
{
    public class StateMachine : GameSystem
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }

        public void ChangeState<T>() 
            where T : class, IState, new()
        {
            var fromState = CurrentState;
            var toState = Container.GetSystem<T>() ?? Container.AddSystem<T>();

            if (fromState != null)
            {
                if (fromState == toState || !fromState.CanTransition(toState))
                {
                    return;
                }
                
                fromState.Exit();
            }

            CurrentState = toState;
            PreviousState = fromState;
            
            toState.Enter();
        }
    }

    public static class StateMachineExtensions
    {
        public static void ChangeState<T>(this IContainer game) 
            where T : class, IState, new()
        {
            var stateMachine = game.GetSystem<StateMachine>();
            stateMachine.ChangeState<T>();
        }
    }
}