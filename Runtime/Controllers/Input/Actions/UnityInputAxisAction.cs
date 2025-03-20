using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Mappings;
using UnityEngine;

namespace BBUnity.Entities.Controllers.Input.Actions {
    
    public class UnityInputAxisAction : UnityInputAction {

        private Vector2 _movement;

        public float X {
            get { return _movement.x; }
        }

        public float Y {
            get { return _movement.y; }
        }

        public UnityInputAxisAction(PlayerInput input, UnityAxisActionMapping mapping) : base(input, mapping) {
            _movement = Vector2.zero;
        }

        public override void Update(float delta) {
            _movement = _action.ReadValue<Vector2>();
        }
    }
}