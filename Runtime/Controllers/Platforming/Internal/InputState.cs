using UnityEngine;

namespace BBUnity.Entities.Controllers.Platforming.Internal {

    public class InputState {

        private float _horizontalMovement;
        private bool _jump;
        private bool _jumpPressed;

        public bool Jump {
            get { return _jump; }
        }

        public bool JumpPressed {
            get { return _jumpPressed; }
        }

        public float HorizontalMovement {
            get { return _horizontalMovement; }
        }

        public bool HasHorizontalMovement {
            get { return _horizontalMovement > float.Epsilon || _horizontalMovement < -float.Epsilon; }
        }

        public void SetHorizontalMovement(float horizontalMovement, bool snapMovement) {
            _horizontalMovement = horizontalMovement;

            if(snapMovement && HasHorizontalMovement) {
                _horizontalMovement = Mathf.Sign(_horizontalMovement);
            }
        }

        public void SetJump(bool jump, bool jumpPressed) {
            _jump = jump;
            _jumpPressed = jumpPressed;
        }
    }
}