using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Mappings;

namespace BBUnity.Entities.Controllers.Input.Actions {
    
    public class UnityInputButtonAction : UnityInputAction {
        
        public UnityInputButtonAction(PlayerInput input, UnityButtonActionMapping mapping) : base(input, mapping) { }

        public virtual bool Down {
            get { return _action.ReadValue<float>() > 0; }
        }

        public virtual bool Pressed {
            get { return _action.WasPressedThisFrame(); }
        }

        public bool Up {
            get { return _action.ReadValue<float>() < float.Epsilon; }
        }

        public bool Released {
            get { return _action.WasReleasedThisFrame(); }
        }
    }
}