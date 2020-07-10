﻿using Castle.Core.Internal;
using System;
using System.Collections.Generic;

namespace Paps.StateMachines.Extensions
{
    public delegate bool ReturnTrueToFinishIteration<T>(T current);

    public static partial class StateMachineExtensions
    {
        public static void AddTransition<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateFrom,
            TTrigger trigger, TState stateTo)
        {
            fsm.AddTransition(new Transition<TState, TTrigger>(stateFrom, trigger, stateTo));
        }

        public static bool ContainsStateByReference<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, IState stateRef)
        {
            TState[] states = fsm.GetStates();

            foreach (TState state in states)
            {
                if (fsm.GetStateById(state) == stateRef)
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetState<T, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm)
        {
            T candidate = default;

            TState[] states = fsm.GetStates();

            foreach (TState state in states)
            {
                if (fsm.GetStateById(state) is T cast)
                {
                    candidate = cast;
                    break;
                }
            }

            return candidate;
        }

        public static T[] GetStates<T, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm)
        {
            List<T> statesList = null;

            TState[] states = fsm.GetStates();

            foreach (TState state in states)
            {
                if (fsm.GetStateById(state) is T cast)
                {
                    if (statesList == null)
                    {
                        statesList = new List<T>();
                    }

                    statesList.Add(cast);
                }
            }

            if (statesList != null)
            {
                return statesList.ToArray();
            }

            return null;
        }

        public static void AddTimerState<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId,
            TimeSpan time, Action onTimerElapsed)
        {
            fsm.AddState(stateId, States.Timer(time, onTimerElapsed));
        }

        public static void AddEmpty<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            fsm.AddState(stateId, States.Empty());
        }

        public static void AddWithEvents<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId,
            Action onEnter, Action onUpdate, Action onExit)
        {
            fsm.AddState(stateId, States.WithEvents(onEnter, onUpdate, onExit));
        }

        public static void AddWithEvents<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId,
            Action onEnter)
        {
            fsm.AddState(stateId, States.WithEvents(onEnter, null, null));
        }

        public static void AddWithEvents<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId,
            Action onEnter, Action onExit)
        {
            fsm.AddState(stateId, States.WithEvents(onEnter, null, onExit));
        }

        public static void AddWithEnterEvent<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, Action onEnter)
        {
            fsm.AddWithEvents(stateId, onEnter, null, null);
        }

        public static void AddWithExitEvent<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, Action onExit)
        {
            fsm.AddWithEvents(stateId, null, null, onExit);
        }

        public static void AddWithUpdateEvent<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, Action onUpdate)
        {
            fsm.AddWithEvents(stateId, null, onUpdate, null);
        }

        public static void ForeachState<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, ReturnTrueToFinishIteration<TState> finishable)
        {
            TState[] states = fsm.GetStates();

            foreach (TState state in states)
            {
                if (finishable(state))
                {
                    return;
                }
            }
        }

        public static void ForeachTransition<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, ReturnTrueToFinishIteration<Transition<TState, TTrigger>> finishable)
        {
            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (Transition<TState, TTrigger> transition in transitions)
            {
                if (finishable(transition))
                {
                    return;
                }
            }
        }

        public static void FromAny<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            TState[] states = fsm.GetStates();

            foreach (TState state in states)
            {
                fsm.AddTransition(new Transition<TState, TTrigger>(state, trigger, stateTo));
            }
        }

        public static void FromAnyExceptTarget<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            TState[] states = fsm.GetStates();

            foreach (TState stateId in states)
            {
                if (stateId.Equals(stateTo) == false)
                {
                    fsm.AddTransition(new Transition<TState, TTrigger>(stateId, trigger, stateTo));
                }
            }
        }

        public static Transition<TState, TTrigger>[] GetTransitionsWithTrigger<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TTrigger trigger)
        {
            List<Transition<TState, TTrigger>> transitionsList = null;

            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.Trigger.Equals(trigger))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<Transition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static Transition<TState, TTrigger>[] GetTransitionsWithStateFrom<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateFrom)
        {
            List<Transition<TState, TTrigger>> transitionsList = null;

            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateFrom))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<Transition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static Transition<TState, TTrigger>[] GetTransitionsWithStateTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateTo)
        {
            List<Transition<TState, TTrigger>> transitionsList = null;

            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateTo.Equals(stateTo))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<Transition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static bool ContainsTransitionWithStateTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateTo)
        {
            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateTo.Equals(stateTo))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsTransitionWithStateFrom<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateFrom)
        {
            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateFrom))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsTransitionWithTrigger<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TTrigger trigger)
        {
            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.Trigger.Equals(trigger))
                {
                    return true;
                }
            }

            return false;
        }

        public static Transition<TState, TTrigger>[] GetTransitionsRelatedTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            List<Transition<TState, TTrigger>> transitionsList = null;

            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateId) || transition.StateTo.Equals(stateId))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<Transition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static bool ContainsTransitionRelatedTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            var transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateId) || transition.StateTo.Equals(stateId))
                {
                    return true;
                }
            }

            return false;
        }

        public static void RemoveAllTransitions<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm)
        {
            Transition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                fsm.RemoveTransition(transition);
            }
        }

        public static void RemoveAllTransitionsRelatedTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            Transition<TState, TTrigger>[] transitions = fsm.GetTransitionsRelatedTo(stateId);

            foreach (var transition in transitions)
            {
                fsm.RemoveTransition(transition);
            }
        }

        public static void ConfigureWithStatesAsTriggersWithNoReentrant<TState>(this IStateMachine<TState, TState> fsm)
        {
            TState[] states = fsm.GetStates();

            for (int i = 0; i < states.Length; i++)
            {
                for (int j = 0; j < states.Length; j++)
                {
                    if (i != j)
                    {
                        fsm.AddTransition(new Transition<TState, TState>(states[i], states[j], states[j]));
                    }
                }
            }
        }

        public static void ConfigureWithStatesAsTriggers<TState>(this IStateMachine<TState, TState> fsm)
        {
            TState[] states = fsm.GetStates();

            for (int i = 0; i < states.Length; i++)
            {
                for (int j = 0; j < states.Length; j++)
                {
                    fsm.AddTransition(new Transition<TState, TState>(states[i], states[j], states[j]));
                }
            }
        }

        public static bool ContainsAll<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, params TState[] stateIds)
        {
            for(int i = 0; i < stateIds.Length; i++)
            {
                if (fsm.ContainsState(stateIds[i]) == false)
                    return false;
            }

            return true;
        }

        public static void AddStates<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, params (TState, IState)[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (fsm.ContainsState(states[i].Item1))
                    throw new StateIdAlreadyAddedException(states[i].Item1.ToString());
            }

            for(int i = 0; i < states.Length; i++)
            {
                fsm.AddState(states[i].Item1, states[i].Item2);
            }
        }

        public static void AddEmptyStates<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, params TState[] states)
        {
            fsm.AddStates(states.ConvertAll(stateId => (stateId, States.Empty() as IState)));
        }

        public static void AddComposite<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, params IState[] states)
        {
            fsm.AddState(stateId, States.Composite(states));
        }
    }
}
