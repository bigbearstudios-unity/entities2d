using UnityEngine;

using System;
using System.Collections.Generic;

using BBUnity.StateMachines;
using BBUnity.Entities.Controllers.Base;
using BBUnity.EditorAttributes;
using BBUnity.Gameplay.Attributes;

using BBUnity.Entities.Controllers.States;

namespace BBUnity.Entities.Controllers {

    sealed public class EntityStateParameter {
        private string _key;
        private EntityState _state;
        private bool _setCurrentState;

        public string Key { get { return _key; } }
        public EntityState State { get { return _state; } }
        public bool SetCurrentState { get { return _setCurrentState; } }

        public EntityStateParameter(string key, EntityState state, bool setCurrentState = false) {
            if (key == null) throw new System.ArgumentNullException("key");
            if (state == null) throw new System.ArgumentNullException("state");

            _key = key;
            _state = state;
            _setCurrentState = setCurrentState;
        }
    }

    public class EntityStateParameters : List<EntityStateParameter> {
        public void Add(string key, EntityState state, bool setCurrentState = false) {
            Add(new EntityStateParameter(key, state, setCurrentState));
        }
    }

    /// <summary>
    /// See <see cref="BBUnity.Entities.Controllers.InputController"/> for the rationale behind
    /// this execution order value and an important caveat about it not being inherited by
    /// concrete subclasses (e.g. PlayerStateController, EnemyStateController).
    /// </summary>
    [AddComponentMenu(""), DefaultExecutionOrder(5)]
    public class StateController : EntityController {

        /// <summary>
        ///
        /// </summary>
        StateMachine _stateMachine = new StateMachine();

        [SerializeField, ReadOnly]
        private string _currentState = "Not Set";

        // Registration (AddState) and initialisation (EntityState.Start()) are kept as separate
        // phases: every state a controller will ever have is registered first, and only once
        // RegisterStates() has fully returned do any of their Start() methods run. This means a
        // state's Start() can safely assume every sibling state on this controller already
        // exists, rather than only the ones registered before it.
        private readonly List<EntityState> _registeredStates = new List<EntityState>();
        private string _initialStateKey;

        protected virtual void RegisterStates() { }

        protected void Start() {
            RegisterStates();

            foreach (EntityState state in _registeredStates) {
                state.Start();
            }

            if (_initialStateKey != null) {
                _stateMachine.SetState(_initialStateKey, true);
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected void Update() {
            _stateMachine.Update();

            if (_stateMachine.CurrentState != null) {
                _currentState = _stateMachine.CurrentState.ReferenceKey;
            }
        }

        /// <summary>
        /// Registers a state with this controller. The state's Start() is deferred until every
        /// state registered via RegisterStates() exists, and setState only takes effect (calling
        /// Enter() on the state machine) after all of those Start() calls have completed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="setState"></param>
        public void AddState(string key, EntityState state, bool setState = false) {
            _stateMachine.AddState(key, state);
            state.SetStateController(this);
            _registeredStates.Add(state);

            if (setState) {
                _initialStateKey = key;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="states"></param>
        public void AddStates(EntityStateParameters stateParameters) {
            foreach (EntityStateParameter p in stateParameters) {
                AddState(p.Key, p.State, p.SetCurrentState);
            }
        }
    }
}