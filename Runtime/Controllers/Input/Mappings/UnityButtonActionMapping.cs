using UnityEngine;
using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Actions;

namespace BBUnity.Entities.Controllers.Input.Mappings {

    [System.Serializable]
    public class UnityButtonActionMapping : UnityActionMapping {

        [SerializeField]
        float _bufferThreshold = 0.0f;

        public float BufferThreshold {
            get { return _bufferThreshold; }
        }

        public UnityButtonActionMapping() {
            _type = MappingTypes.Button;
        }

        /// <summary>
        /// Builds the action. In the case of a button action it can build either a 
        /// UnityInputBufferedButtonAction or UnityInputButtonAction depending on
        /// the inputBufferThreshold
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override UnityInputAction BuildAction(PlayerInput input) {
            if(_bufferThreshold > float.Epsilon) {
                return new UnityInputBufferedButtonAction(input, this);
            } else {
                return new UnityInputButtonAction(input, this);
            }
        }
    }
}