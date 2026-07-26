namespace BBUnity.Entities.Controllers.Input.Actions {

    /// <summary>
    /// A countdown-based input buffer. Call <see cref="Trigger"/> when the source input fires,
    /// and <see cref="Tick"/> once per update with the frame delta. <see cref="IsActive"/>
    /// remains true for the configured threshold after the most recent trigger, allowing a
    /// momentary input to still register a short time later (e.g. a jump pressed just before
    /// landing).
    ///
    /// This is deliberately separate from any specific input source (see
    /// <see cref="UnityInputBufferedButtonAction"/>) so the buffering policy isn't tied to how
    /// the input itself is read.
    /// </summary>
    public class InputBuffer {

        private readonly float _thresholdTime;
        private float _remaining;

        public InputBuffer(float thresholdTime) {
            _thresholdTime = thresholdTime;
            _remaining = 0.0f;
        }

        /// <summary>
        /// Marks the buffer as triggered, restarting its countdown from the threshold.
        /// </summary>
        public void Trigger() {
            _remaining = _thresholdTime;
        }

        /// <summary>
        /// Advances the countdown by the given delta time. Should be called once per update
        /// regardless of whether the buffer was triggered this frame.
        /// </summary>
        public void Tick(float delta) {
            _remaining -= delta;
        }

        /// <summary>
        /// True if the buffer was triggered within the last threshold-time worth of Tick calls.
        /// </summary>
        public bool IsActive {
            get { return _remaining > float.Epsilon; }
        }
    }
}
