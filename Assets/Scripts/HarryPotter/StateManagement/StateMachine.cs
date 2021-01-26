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
                // NOTE: We were previously checking if the states were the same here, but we are no longer doing so to allow cyclical state transitions.
                //       Will this break anything??
                // if (fromState == toState || !fromState.CanTransition(toState)) 
                if (!fromState.CanTransition(toState))
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