
namespace BBUnity.Entities.Controllers.Platforming.States {

    public class CoyoteState {
        private bool _available = false;
        private float _leftGroundAt = float.MinValue;

        public bool Available {
            get { return _available; }
            set { _available = value; }
        }

        public CoyoteState() {
            _leftGroundAt = float.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetLeftGroundAt(float time) {
            _leftGroundAt = time;
        }

        public void Reset() {
            _available = true;
            _leftGroundAt = float.MinValue;
        }

        public void SetAvailable(bool available = true) {
            _available = available;
        }

        public bool AvailableWithinThreshold(float threshold, float time) {
            return _available && (_leftGroundAt + threshold > time);
        }
    }
}