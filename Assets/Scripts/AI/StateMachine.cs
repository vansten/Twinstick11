using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        #region Internal Types

        public abstract class State
        {
            public abstract void OnEnter(T owner);
            public abstract void OnExit(T owner);
            public abstract void Update(T owner, float deltaSeconds);
        }

        #endregion

        #region Variables

        public T Owner;

        protected List<State> _states;
        protected int _currentState;

        #endregion

        #region Methods

        public void Init(T owner)
        {
            Owner = owner;
            _states = new List<State>();
            _currentState = -1;
        }

        public void Update(float deltaSeconds)
        {
            if (_currentState >= 0 && _states.Count > _currentState)
            {
                if (_states[_currentState] != null)
                {
                    _states[_currentState].Update(Owner, deltaSeconds);
                }
            }
        }

        public void ChangeState<S>(bool force = false) where S : State, new()
        {
            System.Type stateType = typeof(S);

            int statesCount = _states.Count;

            if (statesCount > 0 && IsCurrentStateAvailable())
            {
                if (_states[_currentState].GetType() == stateType)
                {
                    if (force)
                    {
                        //Force exit and reenter the state
                        _states[_currentState].OnExit(Owner);
                        _states[_currentState].OnEnter(Owner);
                    }
                    return;
                }
            }

            if (IsCurrentStateAvailable())
            {
                _states[_currentState].OnExit(Owner);
            }

            for (int i = 0; i < statesCount; ++i)
            {
                if (i == _currentState)
                {
                    continue;
                }
                if (_states[i] != null && _states[i].GetType() == stateType)
                {
                    _states[i].OnEnter(Owner);
                    _currentState = i;
                    return;
                }
            }

            //No matching state found, let's create one
            S newState = new S();
            newState.OnEnter(Owner);
            _states.Add(newState);
            _currentState = _states.Count - 1;
        }

        protected bool IsCurrentStateAvailable()
        {
            return _currentState >= 0 && _currentState < _states.Count && _states[_currentState] != null;
        }

        #endregion
    }
}