using UnityEngine;

using System;
using System.Collections.Generic;

using BBUnity.StateMachines;
using BBUnity.Entities.Controllers.Base;
using BBUnity.EditorAttributes;
using BBUnity.Gameplay.Attributes;

using BBUnity.Entities.Controllers.States;

namespace BBUnity.Entities.Controllers {

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

        // Editor-configured states (see StateControllerInspector). This is the preferred way to
        // register states — no subclass/RegisterStates() override required. _defaultState holds
        // a shared reference to whichever entry in _states is marked as the initial state; kept
        // as a reference rather than a list index so it stays correct across reordering.
        [SerializeReference]
        private List<EntityState> _states = new List<EntityState>();

        [SerializeReference]
        private EntityState _defaultState;

        // Registration (AddState) and initialisation (EntityState.Start()) are kept as separate
        // phases: every state a controller will ever have is registered first, and only once
        // RegisterStates() has fully returned do any of their Start() methods run. This means a
        // state's Start() can safely assume every sibling state on this controller already
        // exists, rather than only the ones registered before it.
        private readonly List<EntityState> _registeredStates = new List<EntityState>();
        private string _initialStateKey;

        /// <summary>
        /// Optional extension point for registering states in code, in addition to (or instead
        /// of) the editor-configured list above. Runs after the editor-configured states, so a
        /// state added here with isDefault: true takes precedence over the inspector's default.
        /// </summary>
        protected virtual void RegisterStates() { }

        protected void Start() {
            RegisterSerializedStates();
            RegisterStates();

            foreach (EntityState state in _registeredStates) {
                state.Start();
            }

            if (_initialStateKey != null) {
                _stateMachine.SetState(_initialStateKey, true);
            }
        }

        private void RegisterSerializedStates() {
            foreach (EntityState state in _states) {
                if (state == null) { continue; }

                AddState(state, isDefault: state == _defaultState);
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
        /// Registers a state, deriving its key from its own type (see
        /// <see cref="EntityState.Key{T}"/>) instead of requiring a hand-written key constant.
        /// This is the preferred way to register a state — the string-keyed overload above still
        /// exists because the underlying StateMachine (bbunity-state-machines) is key-based, but
        /// callers should not need to think about keys directly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <param name="isDefault"></param>
        public void AddState<T>(T state, bool isDefault = false) where T : EntityState {
            AddState(EntityState.KeyFor(state.GetType()), state, isDefault);
        }
    }
}