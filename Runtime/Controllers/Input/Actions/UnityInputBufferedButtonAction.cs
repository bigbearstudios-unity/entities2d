using UnityEngine;
using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Mappings;

namespace BBUnity.Entities.Controllers.Input.Actions {
    
    // TODO
    // I think the buffer of inputs shouldn't be part of the input itself
    // but actually somewhere else...
    public class UnityInputBufferedButtonAction : UnityInputButtonAction {

        private float _buffer = 0.0f;
        private float _bufferThreshold = 0.1f;
        
        public UnityInputBufferedButtonAction(PlayerInput input, UnityButtonActionMapping mapping) : base(input, mapping) {
            _buffer = float.MinValue;
            _bufferThreshold = mapping.BufferThreshold;
        }

        public override void Update(float delta) {
            if(_action.WasPressedThisFrame()) {
                _buffer = _bufferThreshold;
            } else {
                _buffer -= delta;
            }
        }

        public override bool Pressed {
            get {
                return _buffer > float.Epsilon; 
            }
        }
    }
}