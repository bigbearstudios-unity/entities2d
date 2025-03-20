using BBUnity.Entities.Controllers.Input.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BBUnity.Entities.Controllers.Input.Mappings {

    public enum MappingTypes {
        Movement,
        Button,
        Axis
    }
    
    [System.Serializable]
    public abstract class UnityActionMapping {
    
        /// <summary>
        /// The name is used to look up this mapping once it has been successfully mapped
        /// onto an action
        /// </summary>
        [SerializeField]
        protected string _action;

        public string Action {
            get { return _action; }
        }

        protected MappingTypes _type;

        public MappingTypes Type {
            get { return _type; }
        }

        public bool IsMovementType {
            get { return _type == MappingTypes.Movement; }
        }

        public bool IsButtonType {
            get { return _type == MappingTypes.Button; }
        }

        public abstract UnityInputAction BuildAction(PlayerInput input);
    }
}
    
    
