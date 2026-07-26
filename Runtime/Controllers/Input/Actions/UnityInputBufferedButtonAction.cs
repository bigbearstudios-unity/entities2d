using UnityEngine;
using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Mappings;

namespace BBUnity.Entities.Controllers.Input.Actions {

    /// <summary>
    /// A button action whose "Pressed" state is buffered: a press remains reported as
    /// "Pressed" for the mapping's buffer threshold, even after the underlying input has
    /// already been released. The buffering policy itself lives in <see cref="InputBuffer"/>;
    /// this class is only responsible for feeding it press events off the real input.
    /// </summary>
    public class UnityInputBufferedButtonAction : UnityInputButtonAction {

        private readonly InputBuffer _buffer;

        public UnityInputBufferedButtonAction(PlayerInput input, UnityButtonActionMapping mapping) : base(input, mapping) {
            _buffer = new InputBuffer(mapping.BufferThreshold);
        }

        public override void Update(float delta) {
            if(_action.WasPressedThisFrame()) {
                _buffer.Trigger();
            }

            _buffer.Tick(delta);
        }

        public override bool Pressed {
            get { return _buffer.IsActive; }
        }
    }
}