
using UnityEngine;

namespace BBUnity.Entities.Controllers {

    /// <summary>
    /// Basic Input Controller. Exposes _horizontal / _vertical movement as standard,
    /// everything else is left up to the developer.
    /// </summary>
    [DefaultExecutionOrder(5)]
    public class InputController : MonoBehaviour {
        protected float _horizontalMovement = 0.0f;
        protected float _verticalMovement = 0.0f;

        public float HorizontalMovement { get { return _horizontalMovement; } }
        public float VerticalMovement { get { return _verticalMovement; } }

        public bool HasHorizontalMovement { get { return _horizontalMovement != 0.0f; } }
        public bool HasVerticalMovement { get { return _verticalMovement != 0.0f; } }

        
    }
}