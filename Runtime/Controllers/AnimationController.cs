using UnityEngine;

using System.Collections.Generic;

using BBUnity.Entities.Controllers.Base;

namespace BBUnity.Entities.Controllers {

    /// <summary>
    /// Controller to aid with animations. The core parts of this is:
    /// - Control of the animator (Helper methods)
    /// - Control of the animation events
    /// </summary>
    public class AnimationController : EntityController {
        internal class InvalidAnimatorException  : System.Exception {
            public InvalidAnimatorException() : base("An Animator component is required for the Animator Controller to function") { }
        }

        private Animator _animator;
        private Dictionary<string, bool> _animationEvents = new Dictionary<string, bool>();

        public bool AnimationComplete {
            get { return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f; }
        }

        /*
         * Unity Functionality
         */

        private void Awake() {
            _animator = GetComponent<Animator>();

            if(_animator == null) {  throw new InvalidAnimatorException(); }
        }

        public void SetEventBoolean(string eventName, bool booleanValue) {
            _animationEvents[eventName] = booleanValue;
        }

        public bool GetEventBoolean(string eventName) {
            _animationEvents.TryGetValue(eventName, out bool value);

            return value;
        }

        /*
         * Animation Callback Functionality
         */ 

        /// <summary>
        /// Sets a given event key to true
        /// </summary>
        /// <param name="name"></param>
        private void SetAnimationEventBooleanTrue(string eventName) {
            SetEventBoolean(eventName, true);
        }

        /// <summary>
        /// Sets a given event key to false
        /// </summary>
        /// <param name="eventName"></param>
        private void SetAnimationEventBooleanFalse(string eventName) {
            SetEventBoolean(eventName, false);
        }

        /*
         * Public API
         */

        /// <summary>
        /// Utility method for calling 'Play' on the animator instance
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        public void Play(string stateName, int layer = -1, float normalizedTime = 0.0f) {
            _animator.Play(stateName, layer, normalizedTime);
        }
    }
}