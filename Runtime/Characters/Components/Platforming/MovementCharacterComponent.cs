using UnityEngine;

using BBUnity.Entities;
using BBUnity.Core2D.Movement;
using BBUnity.Attributes;

namespace BBUnity.Entities.Characters.Components.Platforming {

    public enum FacingDirection {
        Left, // Character is facing the Left
        Right // Character is facing the right
    }

    [System.Serializable]
    public class MovementCharacterComponent : PlatformingCharacterComponent {

        [SerializeField]
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _previousVelocity = Vector3.zero;
        private Vector3 _previousPosition = Vector3.zero;
        private Vector2 _previousSpeed = Vector2.zero;

        [SerializeField]
        private Vector2 _speed = Vector2.zero;

        [Header("Horizontal Movement (Walk / Run)")]

        [SerializeField, Tooltip("The rate at which the object will accelerate from its current horizontal speed")] 
        private float _accelerationRate = 32.0f;

        [SerializeField, Tooltip("The rate at which the object will decelerate from its current horizontal speed")]
        private float _decelerationRate = 160.0f;

        /*
         * 
         */
        [SerializeField, Tooltip("The maximum horizontal speed the object can achieve")] 
        private float _horizontalClampSpeed = 8.0f;

        public float HorizontalClampSpeed {
            get { return _horizontalClampSpeed; }
            set { _horizontalClampSpeed = value; }
        }

        [SerializeField, Tooltip("The speed which is classified as 'Running'")]
        private float _runSpeed = 2.5f;
        //private float 

        [Header("Vertical Movement (Jump / Fall)")]

        [SerializeField] 
        private float _jumpHeight = 15.0f;

        [SerializeField] 
        private float _jumpApexThreshold = 10f;

        [SerializeField, Tooltip("The apex bonus applied to movement. This is applied at the apex of the objects jump")] 
        private float _apexBonus = 2;

        [SerializeField] 
        private float _minimumFallSpeed = 40.0f;

        [SerializeField] 
        private float _maximumFallSpeed = 80.0f;

        public float MaximumFallSpeed {
            get { return _maximumFallSpeed; }
            set { _maximumFallSpeed = value; }
        }

        [SerializeField]
        private float _verticalClampSpeed = -40f;

        [SerializeField] 
        private float _jumpEndEarlyGravityModifier = 3;

        private float _fallSpeed = 0.0f;
        private float _distanceToApexPoint = 0.0f;

        [SerializeField, ReadOnly]
        private bool _endJumpEarly = false;

        [Header("Coyote Time")]

        [SerializeField]
        private CoyoteTime _coyoteTime;

        [Header("Movement Resolution")]
        [SerializeField]
        private uint _positionResolverIterations = 10;

        /*
         * Public Accessors
         * All accessors provide accesses to the core working values which are collected
         * upon first update. This means they should be valid and avalible for use
         * within the callbacks.
         */
        public Vector3 PreviousPosition { get { return _previousPosition; } }
        public bool IsMovingRight { get { return _velocity.x > 0; } }
        public bool IsMovingLeft { get { return _velocity.x < 0; } }
        public bool IsMovingUp { get { return _velocity.y > 0; } }
        public bool IsMovingDown { get { return _velocity.y < 0; } }

        public float AbsoluteHorizonalVelocity { get { return Mathf.Abs(_velocity.x); } }
        public float AbsoluteVerticalVelocity { get { return Mathf.Abs(_velocity.y); } }

        public float HorizontalVelocity { get { return _velocity.x; } }
        public float VerticalVelocity { get { return _velocity.y; } }

        private FacingDirection _direction =  FacingDirection.Right;
        public bool IsFacingRight { get { return _direction == FacingDirection.Right; } }
        public bool IsFacingLeft { get { return _direction == FacingDirection.Left; } }

        private bool HasDelegate { get { return _componentDelegate != null; } }
        private IMovementCharacterComponent MovementDelegate { get { return (IMovementCharacterComponent)_componentDelegate; } }

        public bool CanJump { 
            get { return CollisionComponent.CollidedDown ? true : _coyoteTime.IsAvailable; }
        }

        // TODO
        // On Awake we should check that it has all of the components
        // which it requires to function. E.g. CollisionCharacterComponent
        public override void Awake() {
            Debug.Log("Debug::MovementCharacterComponent->Awake");
        }

        public override void Start() {
            Debug.Log("Debug::MovementCharacterComponent->Start");
        }

        public void ApplyHorizontalMovement(float movement) {
            if(movement > float.Epsilon || movement < -float.Epsilon) { //There is some movement being applied
                float apexBonus = Mathf.Sign(movement) * _apexBonus * _distanceToApexPoint;

                _speed.x += movement * _accelerationRate * Time.deltaTime;
                _speed.x = Mathf.Clamp(_speed.x, -_horizontalClampSpeed, _horizontalClampSpeed);
                _speed.x += apexBonus * Time.deltaTime;
            } else {
                _speed.x = Mathf.MoveTowards(_speed.x, 0, _decelerationRate * Time.deltaTime);
            }

            if(_speed.x > float.Epsilon && CollisionComponent.CollidedRight) {
                _speed.x = 0.0f;
            } 

            if(_speed.x < float.Epsilon && CollisionComponent.CollidedLeft) {
                 _speed.x = 0.0f;
            }
        }

        public void ApplyGravity() {
            _distanceToApexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(_speed.y));
            _fallSpeed = Mathf.Lerp(_minimumFallSpeed, _maximumFallSpeed, _distanceToApexPoint);
            float fallSpeed = _endJumpEarly && _speed.y > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;
            _speed.y -= _fallSpeed * Time.deltaTime;

            if (_speed.y < _verticalClampSpeed) {
                _speed.y = _verticalClampSpeed;
            }
        }

        /// <summary>
        /// Applies the jump to the transform. This consists of:
        /// - Setting endJumpEarly = false
        /// - Setting the speed.y = _jumpheight;
        /// - Resetting the CoyoteTimer
        /// </summary>   
        public void ApplyJump() {
            _endJumpEarly = false;
            _speed.y = _jumpHeight;
            _coyoteTime.Reset();
        }

        // TODO The state should manage the input! (&& InputController.EndJump)
        // TODO This I think can be moved out of here entirely
        public void EndJumpEarly() {
            if(!CollisionComponent.CollidedDown  && _velocity.y > 0) {
                _endJumpEarly = true;
            }
        }

        /// <summary>
        /// Updates the MovementController
        /// </summary>
        public override void Update() {
            if(CollisionComponent.LeftGroundThisFrame) {
                _coyoteTime.StartTimer();
            } 
            
            //TODO Is this still needed??
            // else if(CollisionController.LandedOnGroundThisFrame) {
            //     _coyoteTime.SetAvailable(true);
            // }

            // Limit the movement upwards if we are colliding
            if(CollisionComponent.CollidedUp) { 
                if(_speed.y > 0) {
                    _speed.y = 0;
                }
            }

            // Limit the movement downwards if we are colliding
            if(CollisionComponent.CollidedDown) {
                _speed.y = _speed.y < 0 ? 0 : _speed.y;
                _distanceToApexPoint = 0;
            }

            /*
             * Finally we calculate the changes and perform the callbacks
             */
            CalculateMovementUpdate();

            _previousVelocity = _velocity;
            _velocity = (transform.position - _previousPosition) / Time.deltaTime;
            _previousPosition = transform.position;
        }

        public void SetDelegate(IMovementCharacterComponent controllerDelegate) {
            _componentDelegate = controllerDelegate;
        }

        public void ZeroSpeed() {
            ZeroHorizontalSpeed();
            ZeroVerticalSpeed();
        }

        public void ZeroHorizontalSpeed() {
            _speed.x = 0.0f;
        }

        public void ZeroVerticalSpeed() {
            _speed.y = 0.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        private bool ShouldCalculateMovement {
            get { 
                float ep = float.Epsilon;
                return _speed.x > ep || _speed.x < -ep || _speed.y > ep || _speed.y < -ep;
            }
        }

        private void CalculateMovementUpdate() {
            Vector3 position = transform.position;
            Vector3 moveBy = _speed * Time.deltaTime;
            Vector3 furthestPoint = position + moveBy;

            /*
             * We haven't collided with anything, this is the perfect state and
             * the character can be moved into that position
             */
            Collider2D hit = CollisionComponent.TestOverlapBoxAt(furthestPoint);
            if(!hit) {
                transform.position += moveBy;
                return;
            }

            Vector2 positionToMoveTo = transform.position;
            for(int i = 0; i < _positionResolverIterations; ++i) {
                float step = (float)i / _positionResolverIterations;
                Vector2 positionToTry = Vector2.Lerp(position, furthestPoint, step);

                if(CollisionComponent.TestOverlapBoxAt(furthestPoint)) {
                    transform.position = positionToMoveTo;

                    if(i == 1) {
                        if (_speed.y < 0) {
                            _speed.y = 0;
                        }

                        Vector3 dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * moveBy.magnitude;
                    }
                }

                positionToMoveTo = positionToTry;
            }
        }
    }
}