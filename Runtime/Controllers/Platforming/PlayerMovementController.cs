using UnityEngine;
using System.Collections;

using BBUnity.Entities.Controllers.Base;
using BBUnity.Entities.Controllers.Platforming.Internal;

namespace BBUnity.Entities.Controllers.Platforming {

    /// <summary>
    /// CollisionState
    /// Stores the internal collision state of a given direction. E.g. Up, Down, Left, Right
    /// and allows each of these states to be interrogated
    /// </summary>
    internal sealed class CollisionState {
        bool _currentFrame;
        bool _previousFrame;

        internal void SetState(bool current) {
            _previousFrame = _currentFrame;
            _currentFrame = current;
        }

        public bool CurrentFrame {
            get { return _currentFrame; }
        }

        public bool PreviousFrame {
            get { return _previousFrame; }
        }
    }

    public sealed class MovementState {

        /*
         * The CollisionState for each direction
         */
        private CollisionState _up = new();
        private CollisionState _down = new();
        private CollisionState _right = new();
        private CollisionState _left = new();

        /*
         * The internal velocity, this will be updated every frame
         */
        Vector2 _velocity = Vector2.zero;

        /// <summary>
        /// Internal method to set the current collision states for each direction
        /// </summary>
        /// <param name="up"></param>
        /// <param name="down"></param>
        /// <param name="right"></param>
        /// <param name="left"></param>
        internal void SetCollisionState(bool up = false, bool down = false, bool right = false, bool left = false) {
            _up.SetState(up);
            _down.SetState(down);
            _right.SetState(right);
            _left.SetState(left);
        }

        /// <summary>
        /// Internal method to set the velocity on the state. This will kept it
        /// in-sync with the velocity on the controller.
        /// </summary>
        /// <param name="velocity"></param>
        internal void SetVelocity(Vector2 velocity) {
            _velocity = velocity;
        }

        public bool IsGrounded {
            get { return _down.CurrentFrame; }
        }

        public bool IsAirborne {
            get { return !IsGrounded; }
        }

        public bool HasLanded {
            get { return WasAirBorne && IsGrounded; }
        }

        public bool HasBecomeAirborne {
            get { return WasGrounded && IsAirborne; }
        }

        public bool WasGrounded {
            get { return _down.PreviousFrame; }
        }

        public bool WasAirBorne {
            get { return !WasGrounded; }
        }

        public bool IsHittingHead {
            get { return _up.CurrentFrame; }
        }

        public Vector3 Velocity {
            get { return _velocity; }
        }

        public bool IsMovingHorizontally {
            get { return _velocity.x > float.Epsilon || _velocity.x < -float.Epsilon; }
        }

        public bool IsMovingVertically {
            get { return _velocity.y > float.Epsilon || _velocity.y < -float.Epsilon; }
        }

        public bool IsStandingStill {
            get { return !IsMovingHorizontally; }
        }

        public bool IsRaising {
            get { return _velocity.y > float.Epsilon; }
        }

        public bool IsFalling {
            get { return _velocity.y < -float.Epsilon; }
        }
    }

    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class PlayerMovementController : EntityController {

        /*
         * Configuration
         */

        /* ---------------------------------------------*/
        [Header("Collision")]

        [SerializeField,Tooltip("")]
        private LayerMask _staticLayers;

        [SerializeField, Tooltip("")]
        private LayerMask _platformLayers;

        [SerializeField, Tooltip("")]
        private float _collisionDistance = 0.05f;

        /* ---------------------------------------------*/
        [Header("Hoizontal Movement")]

        [SerializeField, Tooltip("")]
        private float _horizontalMaxmumSpeed = 9.0f;

        [SerializeField, Tooltip("")]
        private float _horizontalAcceleration = 120.0f;

        [SerializeField, Tooltip("")]
        private float _horizontalGroundedDeceleration = 60.0f;

        [SerializeField, Tooltip("")]
        private float _horizontalAirborneDeleration = 30.0f;

        /* ---------------------------------------------*/
        [Header("Movement Modifiers")]

        [SerializeField, Tooltip("")]
        private bool _snapInputMovement = true;

        /* ---------------------------------------------*/
        [Header("Vertical Movement")]

        [SerializeField, Tooltip("")]
        private float _verticalGroundingForce = 0.05f;

        [SerializeField, Tooltip("The force applied in order to 'jump'")]
        private float _verticalJumpForce = 25.0f;

        [SerializeField, Tooltip("")]
        private float _verticalMaximumFallSpeed = 40.0f;

        [SerializeField, Tooltip("")]
        private float _verticalFallAcceleration = 110.0f;

        /* ---------------------------------------------*/
        [Header("Jump Modifiers")]

        [SerializeField, Tooltip("")]
        private float _earlyJumpReleaseGravityModifier = 3;
        private bool _earlyJumpReleaseActive = false;

        [SerializeField, Tooltip("")]
        private float _coyoteTime = 0.15f;

        /* ---------------------------------------------*/
        [Header("Slope Handling")]

        [SerializeField, Tooltip("Extra downward cast distance when already grounded and moving horizontally, to keep contact over downhill slope edges")]
        private float _slopeStickDistance = 0.3f;

        /*
         * Required Unity Components
         * These are required using the attribure RequiredComponents and are needed for the function
         * of this controller.
         */

        private Rigidbody2D _rigidbody;
        private CapsuleCollider2D _capsuleCollider;

        /*
         * Internal State Management
         * This exposes the internal state of the collection / 
         */

        [SerializeField]
        private MovementState _state = new();
        private InputState _inputState = new();

        public MovementState State {
            get { return _state; }
        }

        /*
         * Computed Variables
         */
        [SerializeField]
        private Vector2 _velocity;

        private Vector2 _groundNormal = Vector2.up;


        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>() ?? throw new System.Exception("A 'Rigidbody' component is required");
            _capsuleCollider = GetComponent<CapsuleCollider2D>() ?? throw new System.Exception("A 'CapsuleCollider2D' component is required");
        }

        // This method is going to be pretty bad in terms of size, we might need to consider changing its
        // call signiture
        public void ApplyMovement(
            float horizontalMovement,
            bool jump = false,
            bool jumpPressed = false,
            float horizontalAcceleration = 0.0f,
            bool fallThroughPlatforms = false
            ) {
            _inputState.SetHorizontalMovement(horizontalMovement, _snapInputMovement);
            _inputState.SetJump(jump, jumpPressed);
        }

        public void TogglePlatformCollision(float toogleBackAfter = 0.4f) {
            if (_togglingPlatformCollisions) { return; }

            StartCoroutine(TogglePlatformCollisionsOver(toogleBackAfter));
        }

        private int LayerMaskToLayer(int bitmask) {
            int result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1) {
                bitmask = bitmask >> 1;
                result++;
            }
            return result;
        }

        private bool _togglingPlatformCollisions = false;
        private IEnumerator TogglePlatformCollisionsOver(float waitTime) {
            _togglingPlatformCollisions = true;

            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(_platformLayers), true);
            yield return new WaitForSeconds(waitTime);

            _togglingPlatformCollisions = false;
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(_platformLayers), false);
        }

        public void ApplyZeroHorizontalMovement() {
            _inputState.SetHorizontalMovement(0.0f, false);
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = 0.0f;
        }

        private void FixedUpdate() {
            GatherCollisionState();
            ApplyVerticalMovement();
            ApplyHorizontalMovement();
            ApplyGravity();
            ApplySlopeMovement();

            _rigidbody.linearVelocity = _velocity;

            _state.SetVelocity(_velocity);
        }

        private void GatherCollisionState() {
            bool previousQueriesStartInColliders = Physics2D.queriesStartInColliders;

            Physics2D.queriesStartInColliders = false;

            // Extend the downward check when already grounded and moving horizontally so the
            // character doesn't briefly leave ground when the floor drops away on a downhill slope.
            float groundCheckDist = _collisionDistance;
            if (_state.IsGrounded && Mathf.Abs(_velocity.x) > float.Epsilon) {
                groundCheckDist += _slopeStickDistance;
            }

            RaycastHit2D groundStaticHit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, _capsuleCollider.direction, 0, Vector2.down, groundCheckDist, _staticLayers);
            RaycastHit2D groundPlatformHit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, _capsuleCollider.direction, 0, Vector2.down, groundCheckDist, _platformLayers);
            bool ceilingStaticHit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, _capsuleCollider.direction, 0, Vector2.up, _collisionDistance, _staticLayers);

            _state.SetCollisionState(up: ceilingStaticHit, down: groundStaticHit || groundPlatformHit);

            // Store the surface normal so ApplySlopeMovement can project velocity along slopes.
            if (groundStaticHit) {
                _groundNormal = groundStaticHit.normal;
            } else if (groundPlatformHit) {
                _groundNormal = groundPlatformHit.normal;
            } else {
                _groundNormal = Vector2.up;
            }

            if (_state.IsHittingHead) {
                _velocity.y = Mathf.Min(0, _velocity.y);
            }

            if (_state.IsGrounded) {
                _earlyJumpReleaseActive = false;
            }

            Physics2D.queriesStartInColliders = previousQueriesStartInColliders;
        }

        private void ApplyVerticalMovement() {
            if (_state.IsGrounded) {
                if (_inputState.Jump) {
                    ApplyJumpForce();
                }
            }
        }

        private void ApplyJumpForce() {
            _velocity.y = _verticalJumpForce;
        }

        private void ApplyHorizontalMovement() {
            if (_inputState.HasHorizontalMovement) {
                _velocity.x = Mathf.MoveTowards(_velocity.x, _inputState.HorizontalMovement * _horizontalMaxmumSpeed, _horizontalAcceleration * Time.fixedDeltaTime);
            } else {
                var deceleration = _state.IsGrounded ? _horizontalGroundedDeceleration : _horizontalAirborneDeleration;
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
        }

        private void ApplySlopeMovement() {
            // Skip when airborne, on flat ground, or jumping this frame, (jumping must preserve the full vertical jump force, not the slope's y component).
            if (!_state.IsGrounded || _groundNormal == Vector2.up || _inputState.Jump) {
                return;
            }

            if (Mathf.Abs(_velocity.x) < float.Epsilon) {
                _velocity.x = 0f;
                _velocity.y = -Physics2D.gravity.y * _rigidbody.gravityScale * Time.fixedDeltaTime;
                return;
            }

            // slopeRight is the unit vector pointing along the slope surface in the +x direction.
            // For flat ground (normal = (0,1)) this equals (1,0), so projection is a no-op.
            // For a slope with normal = (-sinθ, cosθ): slopeRight = (cosθ, sinθ) — correctly inclined.
            Vector2 slopeRight = new Vector2(_groundNormal.y, -_groundNormal.x);
            float speed = Mathf.Abs(_velocity.x);
            int direction = _velocity.x > 0 ? 1 : -1;
            _velocity = slopeRight * (speed * direction);
        }

        private void ApplyGravity() {
            if (_state.IsGrounded && _velocity.y <= 0.0f) {
                _velocity.y = _verticalGroundingForce;
            } else {
                var inAirGravity = _verticalFallAcceleration;

                if(!_inputState.JumpPressed && _velocity.y > 0) {
                    inAirGravity *= _earlyJumpReleaseGravityModifier;
                }

                _velocity.y = Mathf.MoveTowards(_velocity.y, -_verticalMaximumFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }
    }
}