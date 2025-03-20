using System;
using UnityEngine;

using BBUnity.Entities.Controllers.Base;

using BBUnity.Entities.Controllers.Platforming.States;
using BBUnity.Entities.Controllers.Platforming.Physics;

namespace BBUnity.Entities.Controllers.Platforming {
    
    /// <summary>
    /// A single controller responsible for all movement on a platforming
    /// based character. This should be used for complicated movements. E.g. Player movement, Boss movement
    /// This class handles the following:
    /// - Collisions
    /// - Movement
    /// 
    /// All inputs to this system are handled externally. E.g. Via the CharacterUnityInputController or
    /// via raw inputs. See examples...
    /// </summary>
    [DefaultExecutionOrder(25)]
    public class CharacterMovementController : EntityController {

        /*
         * Serialized Collision Settings
         */

        [Header("Collision Settings")]

        [SerializeField, Tooltip("The layers which will act as 'Ground'")] 
        private LayerMask _collisionGroundLayers;

        [SerializeField, Tooltip("The layers which will act as 'Platforms'")]
        private LayerMask _collisionPlatformLayers;

        [SerializeField, Range(2, 20), Tooltip("The number of detectors used for collisions")] 
        private int _collisionDetectorCount = 3;

        [SerializeField, Range(0.01f, 0.5f), Tooltip("The length of the detectors")] 
        private float _collisionDetectionRayLength = 0.1f;

        [SerializeField, Range(0.01f, 0.5f), Tooltip("The buffer applied to the rays so that they can shift out of bounds")]
        private float _collisionRayBuffer = 0.01f;

        [SerializeField, Tooltip("The bounds of the collision rectangle")] 
        private Bounds _collisionBounds;

        [Header("Movement Settings - Horizontal")]

        [SerializeField, Tooltip("The maximum horizontal speed the object can achieve. This will act as a clamp on the speed.x")] 
        private float _horizontalMaximumSpeed = 8.0f;

        [SerializeField, Tooltip("The acceleration towards the horizontalMaximumSpeed")]
        private float _horizontalAcceleration = 32.0f;

        [SerializeField, Tooltip("The deceleration from the current speed, back to 0")]
        private float _horizontalDeceleration = 8.0f;

        [SerializeField, Tooltip("The horizontal speed bonus applied when at an apex of a jump")]
        private float _apexHorizontalSpeedBonus = 2.0f;

        [Header("Movement Settings - Vertical")]

        [SerializeField, Tooltip("The minimum speed at which the character can fall vertically")]
        private float _verticalMinimumFallSpeed = 40.0f;

        [SerializeField, Tooltip("The maximum speed at which the character can fall vertically")]
        private float _verticalMaximumFallSpeed = 80.0f;

        [SerializeField, Tooltip("")]
        private float _jumpHeight = 15.0f;

        [SerializeField, Tooltip("The threshold which determines the effect the apex has")] 
        private float _apexThreshold = 10f;

        [SerializeField, Tooltip("The ")]
        private float _jumpReleaseModifier = 3.0f;

        [Header("Movement Settings - Coyote Jump")]

        [SerializeField, Tooltip("The time after leaving the ground that Coyote time should be avalible")]
        private float _coyoteThreshold = 0.1f;

        [Header("Debug Variables")]

        /*
         * Private Collision Variables
         */
        private BoundedRays _collisionRays;
        private CollisionStates _collisionState;

        /*
         * Private Movement Variables
         */
        private CoyoteState _coyoteState;
        private Vector2 _speed = Vector2.zero;

        [SerializeField]
        private float _fallSpeed = 0.0f;

        [SerializeField]
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _previousPosition = Vector3.zero;

        /// <summary>
        /// A value between 0 and 1 representing the distance towards
        /// the apex
        /// </summary>
        [SerializeField]
        private float _apexDistance = 0.0f;

        
        /*
         * Public Collision Properties
         */
        
        public CollisionStates CollisionStates {
            get { return _collisionState; }
        }

        /*
         * Public Movement Properties
         */

        public float HorizontalMaximumspeed {
            get { return _horizontalMaximumSpeed; }
            set { _horizontalMaximumSpeed = value; }
        }

        public float HorizontalAcceleration {
            get { return _horizontalAcceleration; }
            set { _horizontalAcceleration = value; }
        }

        public Vector2 Velocity {
            get { return _velocity; }
        }

        public float AbsoluteHorizonalVelocity { get { return Mathf.Abs(_velocity.x); } }
        public float AbsoluteVerticalVelocity { get { return Mathf.Abs(_velocity.y); } }

        public bool IsMovingHorizontally {
            get { return AbsoluteHorizonalVelocity > float.Epsilon; }
        }

        public bool IsStillHorizontally {
            get { return AbsoluteHorizonalVelocity < float.Epsilon; }
        }

        public bool IsRaising {
            get { return _velocity.y > float.Epsilon; }
        }

        public bool IsFalling {
            get { return _velocity.y < float.Epsilon; }
        }

        /*
         * Internal Unity Messages
         */

        private void Awake() {
            _collisionState ??= new CollisionStates();
            _coyoteState ??= new CoyoteState();
            _collisionRays ??= new BoundedRays();
        }

        private void Start() {}

        private void Update() {

        }

        private void FixedUpdate() {
            _collisionState.SetStates(
                CheckRayCollision(_collisionGroundLayers, _collisionRays.Left),
                CheckRayCollision(_collisionGroundLayers, _collisionRays.Up),
                CheckRayCollision(_collisionGroundLayers, _collisionRays.Right),
                CheckRayCollision(_collisionGroundLayers, _collisionRays.Down) || CheckRayCollision(_collisionPlatformLayers, _collisionRays.Down)
            );

            _collisionRays.Update(_collisionBounds, transform.position, _collisionRayBuffer);

            if(_collisionState.LeftGroundThisFrame) {
                _coyoteState.SetLeftGroundAt(Time.fixedDeltaTime);
            }
            
            if(_collisionState.LandedOnGroundThisFrame) {
                _coyoteState.Reset();
            }

            // Stop walking through the left walls
            if(_collisionState.IsCollidingLeft) {
                if(_speed.x < 0) { _speed.x = 0; }
            }

            // Stop walking through the right walls
            if(_collisionState.IsCollidingRight) {
                if(_speed.x > 0) { _speed.x = 0; }
            }

            // Stop jumping through the ceiling (Or floating through the ceiling)
            if(_collisionState.IsCollidingUp) {
                if(_speed.y > 0) { _speed.y = 0; }
            }

            // Stop falling through the ground
            if(_collisionState.IsCollidingDown) {
                if(_speed.y < 0) {
                    _speed.y = 0;
                }
            } 

            /*
             * Calculating the possible furtherest point that can be moved
             * using the current speed
             */
            Vector3 position = transform.position;
            Vector3 moveBy = _speed * Time.fixedDeltaTime;
            Vector3 furthestPoint = position + moveBy;

            // Collider2D hit = TestOverlapBoxAt(furthestPoint);

            // if(!hit) { // We aren't intersecting with anything, move and return
            //     transform.position += moveBy;
            // }

            transform.position += moveBy;

            _velocity = (transform.position - _previousPosition) / Time.fixedDeltaTime;
            _previousPosition = transform.position;
        }

        private void OnDrawGizmos() {
            
            _collisionRays ??= new BoundedRays();
            _collisionRays.Update(_collisionBounds, transform.position, _collisionRayBuffer);

            if(TestOverlapBoxAt(transform.position)) {
                Gizmos.color = Color.red;
            } else {
                Gizmos.color = Color.yellow;
            }
            Gizmos.DrawWireCube(transform.position + _collisionBounds.center, _collisionBounds.size);

            Gizmos.color = Color.yellow;

            foreach(BoundedRay ray in _collisionRays.All) {
                if(ray.Direction == Vector2.left) {
                    if(_collisionState.IsCollidingLeft) { Gizmos.color = Color.red; }
                }

                if(ray.Direction == Vector2.right) {
                    if(_collisionState.IsCollidingRight) { Gizmos.color = Color.red; }
                }

                if(ray.Direction == Vector2.up) { 
                    if(_collisionState.IsCollidingUp) { Gizmos.color = Color.red; }
                }

                if(ray.Direction == Vector2.down) { 
                    if(_collisionState.IsCollidingDown) { Gizmos.color = Color.red; }
                }

                foreach(Vector2 point in EvaluateCollisionRayPositions(ray)) {
                    Gizmos.DrawRay(point, ray.Direction * _collisionDetectionRayLength);
                }
            }
        }

        /*
         * Movement Public Methods
         */

        public void ApplyZeroHorizontalMovement() {
            _speed.x = 0.0f;
        }

        [SerializeField]
        private float _calculatedFallSpeed = 0.0f;
        private bool _allowEndJumpEarly = false;
        private bool _endJumpEarly = false;
        public void ApplyMovement(float movement, bool jump = false, bool endJumpEarly = false, float horizontalAcceleration = 0.0f) {

            if(_allowEndJumpEarly && endJumpEarly) {
                _allowEndJumpEarly = false;
                _endJumpEarly = true;
            }

            // Calculate the apex point
            if(CollisionStates.IsInAir) {
                _apexDistance = Mathf.InverseLerp(_apexThreshold, 0, Mathf.Abs(_velocity.y));
                _fallSpeed = Mathf.Lerp(_verticalMinimumFallSpeed, _verticalMaximumFallSpeed, _apexDistance);
            } else { // Make sure to 0 the apex point if we are on the ground, fall speed will be zero'ed anyway
                _apexDistance = 0.0f;
            }

            // Handle the movement first, 
            if(movement > float.Epsilon || movement < -float.Epsilon) {
                if(_speed.x < float.Epsilon && movement > float.Epsilon) { // Moving left, wanting to move right
                    _speed.x = 0.0f;
                }

                if(_speed.x > float.Epsilon && movement < -float.Epsilon) { // Moving left, wanting to move right
                    _speed.x = 0.0f;
                }

                float acceleration = (horizontalAcceleration > float.Epsilon) ? horizontalAcceleration : _horizontalAcceleration;

                // Calculate the speed based on the accelleration, then clamp it too the maximum
                _speed.x += movement * acceleration * Time.deltaTime;
                _speed.x = Mathf.Clamp(_speed.x, -_horizontalMaximumSpeed, _horizontalMaximumSpeed);

                // Calculate the bonus applied by the movement apex, then apply it
                var apexBonus = Mathf.Sign(movement) * _apexHorizontalSpeedBonus * _apexDistance;
                _speed.x += apexBonus * Time.deltaTime;

            } else { // slow the player down, if there isn't any movement. E.g. Deceleration
                _speed.x = Mathf.MoveTowards(_speed.x, 0, _horizontalDeceleration * Time.deltaTime);
            }

            // Calculate and apply gravity, unless we are grounded
            if(_collisionState.IsCollidingDown) {
                _endJumpEarly = false;
                if(_speed.y < 0) { _speed.y = 0; }
            } else {
                _calculatedFallSpeed = _endJumpEarly && _speed.y > float.Epsilon ? _fallSpeed * _jumpReleaseModifier : _fallSpeed;
                _speed.y -= _calculatedFallSpeed * Time.deltaTime;

                if (_speed.y < -_verticalMaximumFallSpeed) { // Clamp the fall speed to the maximum it can be
                    _speed.y = -_verticalMaximumFallSpeed;
                }
            }

            // Calculate the jump, this might override Gravity which is why its next inline
            if(jump && (_collisionState.IsCollidingDown || _coyoteState.AvailableWithinThreshold(_coyoteThreshold, Time.time))) {
                _allowEndJumpEarly = true;

                _speed.y = _jumpHeight;
                _coyoteState.SetAvailable(false); // We have used the coyote jump
            }
        }

        /*
         * Collision Private Methods
         */

        private Collider2D TestOverlapBoxAt(Vector3 point) {
            return Physics2D.OverlapBox(point + _collisionBounds.center, _collisionBounds.size, 0, _collisionGroundLayers);
        }

        private bool CheckRayCollision(LayerMask against, BoundedRay ray) {
            foreach(Vector2 point in EvaluateCollisionRayPositions(ray)) {
                if(Physics2D.Raycast(point, ray.Direction, _collisionDetectionRayLength, against)) {
                    return true;
                }
            }

            return false;
        }

        private System.Collections.Generic.IEnumerable<Vector2> EvaluateCollisionRayPositions(BoundedRay ray) {
            for (int i = 0; i < _collisionDetectorCount; i++) {
                yield return Vector2.Lerp(ray.Start, ray.End, (float)i / (_collisionDetectorCount - 1));
            }
        }
    }
}