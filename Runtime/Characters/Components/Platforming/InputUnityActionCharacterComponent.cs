using UnityEngine;
using UnityEngine.InputSystem;
using BBUnity.Entities.Utilities;

namespace BBUnity.Entities.Characters.Components.Platforming {

    /// <summary>
    /// TODO
    /// This entire component needs cleaning up!
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
        private InputAction _heavyAttackAction = null;

        // // TODO 
        // // Move these to be assignable via the Input
        // // Actions. E.g. Checkbox -> bufferable, int -> threashold
        [SerializeField]
        private InputBuffer _jumpBuffer;

        [SerializeField]
        private InputBuffer _attackBuffer;

        [SerializeField]
        private InputBuffer _heavyAttackBuffer;

        public override void Awake() {

        }

        public override void Start() {
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            _attackAction = _playerInput.actions["Attack"];
            _heavyAttackAction = _playerInput.actions["HeavyAttack"];
        }

        public override void Update() {
            float deltaTime = Time.deltaTime;

            Vector2 movement = Vector2For(_moveAction);

            _horizontalMovement = movement.x;
            _verticalMovement = movement.y;

            _attack = _attackBuffer.Update(GetButtonDown(_attackAction), deltaTime);
            _heavyAttack = _heavyAttackBuffer.Update(GetButtonDown(_heavyAttackAction), deltaTime);
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