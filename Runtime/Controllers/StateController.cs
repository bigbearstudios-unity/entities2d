using UnityEngine;

using System.Collections.Generic;

using BBUnity.StateMachines;
using BBUnity.Entities.Controllers.Base;
using BBUnity.EditorAttributes;
using BBUnity.Gameplay.Attributes;

namespace BBUnity.Entities.Controllers {

    public class EntityState : State {

        protected StateController _stateController;

        public StateController StateController {
            get { return _stateController; }
        }

        internal void SetStateController(StateController controller) {
            _stateController = controller;
        }
        
        protected T GetComponent<T>() {
            return _stateController.GetComponent<T>();
        }

        public virtual void Start() {}
    }

    sealed public class EntityStateParameter {
        private string _key;
        private EntityState _state;
        private bool _setCurrentState;

        public string Key { get { return _key; } }
        public EntityState State { get { return _state; } }
        public bool SetCurrentState { get { return _setCurrentState; } }

        public EntityStateParameter(string key, EntityState state, bool setCurrentState = false) {
            if(key == null) throw new System.ArgumentNullException("key");
            if(state == null) throw new System.ArgumentNullException("state");

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

    // TODO
    // We should figure out a way to remove the ability to add 'State' to the state machine
    // here and enforce the 'EntityState' being required

    [AddComponentMenu(""), DefaultExecutionOrder(5)]
    public class StateController : EntityController {

        /// <summary>
        /// 
        /// </summary>
        StateMachine _stateMachine = new StateMachine();

        [SerializeField, ReadOnly]
        private string _currentState = "Not Set";

        protected virtual void RegisterStates() {}

        protected void Start() {
            RegisterStates();
        }

        /// <summary>
        ///
        /// </summary>
        protected void Update() {
            _stateMachine.Update();

            if(_stateMachine.CurrentState != null) {
                _currentState = _stateMachine.CurrentState.ReferenceKey;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="setState"></param>
        public void AddState(string key, EntityState state, bool setState = false) {
            _stateMachine.AddState(key, state);

            state.SetStateController(this);
            state.Start();

            if(setState) {
                _stateMachine.SetState(key, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="states"></param>
        public void AddStates(EntityStateParameters stateParameters) {
            foreach(EntityStateParameter p in stateParameters) {
                AddState(p.Key, p.State, p.SetCurrentState);
            }
        }
    }
}