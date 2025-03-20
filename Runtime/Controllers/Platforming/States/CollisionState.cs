
namespace BBUnity.Entities.Controllers.Platforming.States {

    public class CollisionState {

        /// <summary>
        /// 
        /// </summary>
        private bool _currentFrame = false;

        /// <summary>
        /// 
        /// </summary>
        private bool _previousFrame = false;

        public bool CurrentFrame {
            get { return _currentFrame; }
        }

        public bool PreviousFrame {
            get { return _previousFrame; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFrame"></param>
        public void SetState(bool newFrame) {
            _previousFrame = _currentFrame;
            _currentFrame = newFrame;
        }
    }

    public class CollisionStates {

        public CollisionState[] _states;

        public CollisionStates() {
            _states = new CollisionState[] {
                new CollisionState(),
                new CollisionState(),
                new CollisionState(),
                new CollisionState()
            };
        }

        // private bool _left = false;
        // private bool _up = false;
        // private bool _right = false;
        // private bool _down = false;

        // private bool _leftLastFrame = false;
        // private bool _upLastFrame = false;
        // private bool _rightLastFrame = false;
        // private bool _downLastFrame = false;

        public void SetStates(bool left, bool up, bool right, bool down) {
            _states[0].SetState(left);
            _states[1].SetState(up);
            _states[2].SetState(right);
            _states[3].SetState(down);
        }

        public CollisionState Left {
            get { return _states[0]; }
        }

        public CollisionState Up {
            get { return _states[1]; }
        }

        public CollisionState Right {
            get { return _states[2]; }
        }

        public CollisionState Down {
            get { return _states[3]; }
        }

        public bool IsCollidingDown {
            get { return Down.CurrentFrame; }
        }

        public bool IsCollidingLeft {
            get { return Left.CurrentFrame; }
        }

        public bool IsCollidingRight {
            get { return Right.CurrentFrame; }
        }

        public bool IsCollidingUp {
            get { return Up.CurrentFrame; }
        }

        public bool IsInAir {
            get { return !Down.CurrentFrame; }
        }

        public bool WasGrounded {
            get { return Down.PreviousFrame; }
        }

        public bool WasInAir {
            get { return !Down.PreviousFrame; }
        }

        public bool LeftGroundThisFrame {
            get { return WasGrounded && IsInAir; }
        }

        public bool LandedOnGroundThisFrame {
            get { return IsCollidingDown && WasInAir; }
        }
    }
}