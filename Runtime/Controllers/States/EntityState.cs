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
    }
}