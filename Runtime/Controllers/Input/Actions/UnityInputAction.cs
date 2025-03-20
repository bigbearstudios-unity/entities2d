using UnityEngine;
using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Mappings;

namespace BBUnity.Entities.Controllers.Input.Actions {

    /// <summary>
    /// The base InputAction. Does nothing on its own apart from expose the bare functionality
    /// on the Unity InputAction
    /// </summary>
    public abstract class UnityInputAction {
        
        protected InputAction _action;

        public UnityInputAction(PlayerInput input, UnityActionMapping mapping) {
            _action = input.actions[mapping.Action];
        }

        public virtual void Update(float delta) {}
    }
}