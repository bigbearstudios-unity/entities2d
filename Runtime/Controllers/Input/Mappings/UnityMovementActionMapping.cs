using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Actions;

namespace BBUnity.Entities.Controllers.Input.Mappings {

    [System.Serializable]
    public class UnityMovementActionMapping : UnityAxisActionMapping {

        public UnityMovementActionMapping() {
            _action = "Movement";
            _type = MappingTypes.Movement;
        }

        public override UnityInputAction BuildAction(PlayerInput input) {
            return new UnityInputAxisAction(input, this);
        }
    }
}