﻿using System.Collections.Generic;

namespace Paps.StateMachines
{
    public class TransitionEqualityComparer<TState, TTrigger> : IEqualityComparer<Transition<TState, TTrigger>>
    {
        private IEqualityComparer<TState> _stateComparer;
        private IEqualityComparer<TTrigger> _triggerComparer;

        public TransitionEqualityComparer(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;
        }

        public bool Equals(Transition<TState, TTrigger> x, Transition<TState, TTrigger> y)
        {
            return _stateComparer.Equals(x.StateFrom, y.StateFrom) &&
                _triggerComparer.Equals(x.Trigger, y.Trigger) &&
                _stateComparer.Equals(x.StateTo, y.StateTo);
        }

        public int GetHashCode(Transition<TState, TTrigger> obj)
        {
            return (obj.StateFrom, obj.Trigger, obj.StateTo).GetHashCode();
        }
    }
}
