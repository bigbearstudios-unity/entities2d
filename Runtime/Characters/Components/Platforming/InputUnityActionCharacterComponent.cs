using UnityEngine;
using UnityEngine.InputSystem;
using BBUnity.Entities.Utilities;

namespace BBUnity.Entities.Characters.Components.Platforming {

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class InputUnityActionCharacterComponent : InputCharacterComponent {

        // /// <summary>
        // /// The Unity Player Input
        // /// </summary>
        [SerializeField]
        private PlayerInput _playerInput = null;

        // /*
        //  * Input Actions
        //  */
        private InputAction _moveAction = null;
        private InputAction _jumpAction = null;
        private InputAction _attackAction = null;

        // // TODO 
        // // Move these to be assignable via the Input
        // // Actions. E.g. Checkbox -> bufferable, int -> threashold
        [SerializeField]
        private InputBuffer _jumpBuffer;

        [SerializeField]
        private InputBuffer _attackBuffer;

        public override void Awake() {
            Debug.Log("Debug::InputUnityActionCharacterComponent->Awake");

            // TODO
            // Check we have an input registered...Could we maybe find the
            // component on the object?
        }

        public override void Start() {
            Debug.Log("Debug::InputUnityActionCharacterComponent->Start");

            // TODO
            // Each of the required inputs should be checked
            // and a debug message sent to the console
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            _attackAction = _playerInput.actions["Attack"];
        }

        public override void Update() {
            float deltaTime = Time.deltaTime;

            Vector2 movement = Vector2For(_moveAction);

            _horizontalMovement = movement.x;
            _verticalMovement = movement.y;

            _attack = _attackBuffer.Update(GetButtonDown(_attackAction), deltaTime);
            _jump = _jumpBuffer.Update(GetButtonDown(_jumpAction), deltaTime);

            _endJump = GetButtonUp(_jumpAction);
            _endAttack = GetButtonUp(_attackAction);
        }

        private Vector2 Vector2For(InputAction action) {
            return action.ReadValue<Vector2>();
        }

        private bool GetButtonDown(InputAction action) {
            return action.triggered && action.ReadValue<float>() > 0;
        }

        private bool GetButtonUp(InputAction action) {
            return action.triggered && action.ReadValue<float>() == 0;
        }
    }
}