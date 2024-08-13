using UnityEngine;
using BBUnity.Physics2D;

namespace BBUnity.Entities.Characters.Components.Platforming {


    // TODO Move this back to the main component
    [System.Serializable]
    public class CollisionCharacterComponentSettings {
        [SerializeField, Tooltip("The bounds of the collision rectangle")] 
        public Bounds bounds;

        [SerializeField, Tooltip("The layers which are classed as the ground")] 
        public LayerMask groundLayers;

        [SerializeField, Range(3, 20), Tooltip("The number of detectors used for collisions")] 
        public int detectorCount = 3;

        [SerializeField, Range(0.1f, 0.5f), Tooltip("The length of the detectors")] 
        public float detectionRayLength = 0.1f;

        [SerializeField, Range(0.01f, 0.3f), Tooltip("The ray buffer. This is the distance of the bounding rays")] 
        public float rayBuffer = 0.1f;
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class CollisionCharacterComponent : PlatformingCharacterComponent {

        [SerializeField]
        private CollisionCharacterComponentSettings _settings;

        private BoundedRays _collisionRays;

        private bool _collidedLeftLastFrame = false;
        private bool _collidedUpLastFrame = false;
        private bool _collidedRightLastFrame = false;
        private bool _collidedDownLastFrame = false;

        [Header("Collision State")]
        [SerializeField]
        private bool _collidedLeft = false;

        [SerializeField]
        private bool _collidedUp = false;

        [SerializeField]
        private bool _collidedRight = false;

        [SerializeField]
        private bool _collidedDown = false;

        public bool CollidedLeftLastFrame { get { return _collidedLeftLastFrame; } }
        public bool CollidedUpLastFrame { get { return _collidedUpLastFrame; } }
        public bool CollidedRightLastFrame { get { return _collidedRightLastFrame; } }
        public bool CollidedDownLastFrame { get { return _collidedDownLastFrame; } }

        public bool CollidedLeft { get { return _collidedLeft; } }
        public bool CollidedUp { get { return _collidedUp; } }
        public bool CollidedRight { get { return _collidedRight; } }
        public bool CollidedDown { get { return _collidedDown; } }

        public bool LeftGroundThisFrame { get { return _collidedDownLastFrame && !_collidedDown; } }
        public bool LandedOnGroundThisFrame { get { return !_collidedDownLastFrame && _collidedDown; } }

        public Bounds Bounds { get { return _settings.bounds; } }
        public LayerMask GroundLayers { get { return _settings.groundLayers; } }
        public Vector3 BoundedPosition { get { return transform.position + _settings.bounds.center; } }

        private ICollisionCharacterComponent CollisionDelegate { get { return (ICollisionCharacterComponent)_componentDelegate; } }

        public override void Awake() {
            _collisionRays = new BoundedRays(Bounds);

            //TODO
            // Check we have some collision settings as these would be needed
            // for all games!
        }

        public override void Start() {

        }

        public override void Update() {
            UpdateCollisionRays();
            UpdateCollisions();
            // PerformCallbacks();
        }

        public virtual void FixedUpdate() {
            
        }

        // public void SetDelegate(ICollisionComponent controllerDelegate) {
        //     _controllerDelegate = controllerDelegate;
        // }

        public Collider2D TestOverlapBoxAt(Vector3 point) {
            return UnityEngine.Physics2D.OverlapBox(point + Bounds.center, Bounds.size, 0, GroundLayers);
        }

        private void UpdateCollisionRays() {
            _collisionRays.Update(transform.position, _settings.rayBuffer);
        }

        private void UpdateCollisions() {
            _collidedLeftLastFrame = _collidedLeft;
            _collidedUpLastFrame = _collidedUp;
            _collidedRightLastFrame = _collidedRight;
            _collidedDownLastFrame = _collidedDown;

            _collidedLeft = CheckCollisionFor(_collisionRays.Left);
            _collidedUp = CheckCollisionFor(_collisionRays.Up);
            _collidedRight = CheckCollisionFor(_collisionRays.Right);
            _collidedDown = CheckCollisionFor(_collisionRays.Down);
        }

        private bool CheckCollisionFor(BoundedRay ray) {
            foreach(Vector2 point in EvaluateRayPositions(ray)) {
                if(UnityEngine.Physics2D.Raycast(point, ray.Direction, _settings.detectionRayLength, GroundLayers)) {
                    return true;
                }
            }

            return false;
        }

        private System.Collections.Generic.IEnumerable<Vector2> EvaluateRayPositions(BoundedRay ray) {
            for (int i = 0; i < _settings.detectorCount; i++) {
                yield return Vector2.Lerp(ray.Start, ray.End, (float)i / (_settings.detectorCount - 1));
            }
        }

        private void PerformCallbacks() {
            // if(LeftGroundThisFrame) {
            //     CollisionDelegate?.LeftGround();
            // } else if(LandedOnGroundThisFrame) {
            //     CollisionDelegate?.LandedOnGround();
            // }
        }

        [Header("Gizmos")]
        public bool _renderBoundingGizmo = false;
        public bool _renderRayGizmo = false;

        public override void OnDrawGizmos() {
            if(_renderBoundingGizmo) {
                if(UnityEngine.Physics2D.OverlapBox(transform.position + _settings.bounds.center, _settings.bounds.size, 0, GroundLayers)) {
                    Gizmos.color = Color.red;
                } else {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawWireCube(transform.position + _settings.bounds.center, _settings.bounds.size);
            }

            if(_renderRayGizmo) {
                _collisionRays = new BoundedRays(_settings.bounds);

                UpdateCollisionRays();

                Gizmos.color = Color.yellow;
                foreach(BoundedRay ray in _collisionRays.All) {
                    foreach(Vector2 point in EvaluateRayPositions(ray)) {
                        Gizmos.DrawRay(point, ray.Direction * _settings.detectionRayLength);
                    }
                }
            }
        }
    }
}