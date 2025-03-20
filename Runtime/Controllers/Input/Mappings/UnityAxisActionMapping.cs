using UnityEngine.InputSystem;

using BBUnity.Entities.Controllers.Input.Actions;

namespace BBUnity.Entities.Controllers.Input.Mappings {

    [System.Serializable]
    public class UnityAxisActionMapping : UnityActionMapping {

        public UnityAxisActionMapping() {
            _type = MappingTypes.Axis;
        }

        public override UnityInputAction BuildAction(PlayerInput input) {
            return new UnityInputAxisAction(input, this);
        }
    }
}