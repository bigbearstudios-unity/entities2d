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

        Animator _animator;
        Dictionary<string, bool> _animationEvents = new Dictionary<string, bool>();

        public bool AnimationComplete {
            get { return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f; }
        }

        /*
         * Unity Functionality
         */

        private void Awake() {
            _animator = GetComponent<Animator>();
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

        public void Play(string stateName) {
            _animator.Play(stateName);
        }
    }
}