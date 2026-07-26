
using UnityEngine;

namespace BBUnity.Entities.Controllers {

    /// <summary>
    /// Basic Input Controller. Exposes _horizontal / _vertical movement as standard,
    /// everything else is left up to the developer.
    ///
    /// Execution order intent: input must be readable before any <see cref="StateController"/>
    /// queries it inside a state's Start()/Update(), and both must run before
    /// <see cref="BBUnity.Gameplay.Attributes.AttributeController"/> resets its attributes — hence
    /// InputController/StateController = 5, AttributeController = 10.
    ///
    /// IMPORTANT: DefaultExecutionOrder is NOT inherited by subclasses. Concrete input
    /// controllers (e.g. PlayerUnityInputController) and state controllers (e.g.
    /// PlayerStateController, EnemyStateController) — the types actually attached to
    /// GameObjects — do not currently redeclare this attribute, so in practice they run in
    /// Unity's default order-0 group rather than at 5/10. Any subclass that needs to honour
    /// this ordering must apply its own [DefaultExecutionOrder] attribute; the value here only
    /// affects components typed exactly as InputController/StateController/AttributeController.
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