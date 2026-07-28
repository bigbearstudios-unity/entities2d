using System;

using BBUnity.StateMachines;

namespace BBUnity.Entities.Controllers.States {
    [Serializable]
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

        public virtual void Start() { }

        /// <summary>
        /// Transitions the owning StateController to the state registered for type T. The
        /// underlying StateMachine (bbunity-state-machines) is still purely string-keyed — see
        /// <see cref="Key{T}"/> for how that key is derived.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="forceTransition"></param>
        protected void SetState<T>(bool forceTransition = false) where T : EntityState {
            SetState(Key<T>(), forceTransition);
        }

        /// <summary>
        /// The string key a state of type T is registered under — the type's full name
        /// (namespace + type), so it stays stable across list reordering and doesn't require a
        /// hand-written constant per state class.
        /// </summary>
        public static string Key<T>() where T : EntityState {
            return KeyFor(typeof(T));
        }

        internal static string KeyFor(Type stateType) {
            return stateType.FullName;
        }
    }
}