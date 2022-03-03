using System;
using System.Collections.Generic;
using HarryPotter.Systems.Core;

namespace HarryPotter.StateManagement
{
    public class StateMachine : GameSystem
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }


        private readonly Dictionary<Type, IState> _states;

        public StateMachine()
        {
            _states = new Dictionary<Type, IState>();
        }

        public void AddState(IState newState)
        {
            newState.Owner = this;
            newState.Game = Container;

            _states.Add(newState.GetType(), newState);
        }

        public void ChangeState<TState>()
            where TState : class, IState, new()
        {
            var fromState = CurrentState;

            if (!_states.TryGetValue(typeof(TState), out var toState))
            {
                toState = new TState
                {
                    Owner = this,
                    Game = Container
                };

                _states.Add(typeof(TState), toState);
            }

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
        public static void ChangeState<TState>(this IContainer game)
            where TState : class, IState, new()
        {
            var stateMachine = game.GetSystem<StateMachine>();
            stateMachine.ChangeState<TState>();
        }
    }
}
