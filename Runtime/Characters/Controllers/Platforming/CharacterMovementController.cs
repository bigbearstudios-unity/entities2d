using System;
using UnityEngine;

using BBUnity.Entities.Controllers.Base;

namespace BBUnity.Entities.Characters.Controllers.Platforming {
    
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
    [Serializable]
    public class CharacterMovementController : EntityController {

        [Header("Collision Settings")]

        [SerializeField, Tooltip("The layers which the controller will check for collisions with")] 
        private LayerMask _collisionLayers;

        [SerializeField, Range(3, 20), Tooltip("The number of detectors used for collisions")] 
        private int _collisionDetectorCount = 3;

        [SerializeField, Range(0.1f, 0.5f), Tooltip("The length of the detectors")] 
        public float _collisionDetectionRayLength = 0.1f;

        [SerializeField, Range(0.01f, 0.3f), Tooltip("The ray buffer. This is the distance of the bounding rays")] 
        public float _collisionRayBuffer = 0.01f;

        [SerializeField, Tooltip("The bounds of the collision rectangle")] 
        private Bounds _collisionBounds;

        [Header("Movement Settings")]

        [SerializeField, Tooltip("The rate at which the object will accelerate from its current horizontal speed")] 
        private float _movementHorizontalAcceleration = 32.0f;

        [SerializeField, Tooltip("The maximum horizontal speed the object can achieve")] 
        private float _movementMaximumHorizontalSpeed = 8.0f;

        private void Awake() {

        }

        private void Start() {

        }

        private void Update() {

        }

        private void OnDrawGizmos() {

        }
    }
}