using UnityEngine;

namespace BBUnity.Movement {

    [System.Serializable]
    /// <summary>
    /// A serializable CoyoteTime class which has the following functionality:
    /// - Allows the resetting of the timer
    /// - Allows access to the 'IsAvailable' boolean value
    /// </summary>
    public class CoyoteTime {

        [SerializeField, Tooltip("The threshold for Coyote time. This is the amount of time in which coyote time is available.")] 
        private float _thresholdTime = 0.1f;

        private bool _isAvailable = false;
        private float _startedAt = float.MinValue;

        public CoyoteTime() {
            Reset();
        }

        /// <summary>
        /// Resets the CoyoteTime, this sets avalible to false and startedAt
        /// to a 0.0..
        /// </summary>
        public void Reset() {
            _isAvailable = false;
            _startedAt = float.MinValue;
        }

        /// <summary>
        /// Starts the CoyoteTimer
        /// </summary>
        public void StartTimer() {
            _startedAt = Time.time;
        }

        /// <summary>
        /// Sets the CoyoteTimer to avalible
        /// </summary>
        /// <param name="available"></param>
        public void SetAvailable(bool available) {
            _isAvailable = available;
        }

        /// <summary>
        /// Returns a boolean value if the CoyoteTime is available
        /// </summary>
        /// <value></value>
        public bool IsAvailable {
           get { return _isAvailable && (Time.time - _startedAt) < _thresholdTime; } 
        }
    }    
}