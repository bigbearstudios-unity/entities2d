using UnityEngine;

namespace BBUnity.Entities.Utilities {

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class InputBuffer {

        [SerializeField, Tooltip("")]
        private float _thresholdTime = 0.1f;
        private float _timeRemaining = 0.0f;

        /// <summary>
        /// Contructs the basic input buffer. Set the 
        /// _timeRemaining to float.MinValue
        /// </summary>
        public InputBuffer() {
            _timeRemaining = float.MinValue;
        }

        /// <summary>
        /// Updates the input buffer. Returns true / false
        /// if the buffered timeRemaining is above 0 once
        /// the delta has been removed
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool Update(bool input, float delta) {
            if(input) {
                _timeRemaining = _thresholdTime;
            } else {
                _timeRemaining -= delta;
            }

            return _timeRemaining > 0;
        }
    }
}