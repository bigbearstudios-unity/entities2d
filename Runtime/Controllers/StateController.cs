using UnityEngine;

using System.Collections.Generic;

using BBUnity.StateMachines;
using BBUnity.Entities.Controllers.Base;
using BBUnity.Gameplay.Attributes;

using BBUnity.Entities.Characters.Components.Platforming;

namespace BBUnity.Entities.Controllers {

    /// <summary>
    /// 
    /// </summary>
    public class EntityState : State {

        private StateController _controller;

        protected AnimationController AnimationController {
            get { return _controller.animationController; }
        }

        protected AttributeController AttributeController {
            get { return _controller.attributeController; }
        }

        protected EffectController EffectController {
            get { return _controller.effectController; }
        }

        internal void SetStateController(StateController controller) {
            _controller = controller;
        }

        protected T FindController<T>() {
            return _controller.GetComponent<T>();
        }
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


    /// <summary>
    /// Statemachine Controller
    /// </summary>
    [AddComponentMenu("")]
    public class StateController : EntityController {

        /// <summary>
        /// 
        /// </summary>
        StateMachine _stateMachine = new StateMachine();

        internal AnimationController animationController;
        internal AttributeController attributeController;
        internal EffectController effectController;

        private void Awake() {
            animationController = GetComponent<AnimationController>();
            attributeController = GetComponent<AttributeController>();
            effectController = GetComponent<EffectController>();
        }

        private void Start() {
            Initialize();
        }

        /// <summary>
        ///
        /// </summary>
        private void Update() {
            _stateMachine.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="setState"></param>
        public void AddState(string key, EntityState state, bool setState = false) {
            state.SetStateController(this);

            _stateMachine.AddState(key, state, setState);
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