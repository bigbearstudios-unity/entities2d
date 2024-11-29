
namespace BBUnity.Entities.Characters.Components.Platforming {

    /// <summary>
    /// A base input component for all platforming games.
    /// </summary>
    public class InputCharacterComponent : PlatformingCharacterComponent {
        protected float _horizontalMovement = 0.0f;
        protected float _verticalMovement = 0.0f;

        protected bool _jump = false;
        protected bool _endJump = false;

        protected bool _roll = false;

        protected bool _attack = false;
        protected bool _heavyAttack = false;
        protected bool _endAttack = false;

        public float HorizontalMovement { get { return _horizontalMovement; } }
        public float VerticalMovement { get { return _verticalMovement; } }

        public bool HasHorizontalMovement { get { return _horizontalMovement != 0.0f; } }
        public bool HasVerticalMovement { get { return _verticalMovement != 0.0f; } }

        public bool Jump { get { return _jump; } }
        public bool EndJump { get { return _endJump; } }

        public bool Roll { get { return _roll; } }

        public bool Attack { get { return _attack; } }
        public bool HeavyAttack { get { return _heavyAttack; } }
        public bool EndAttack { get { return _endAttack; } }
    }
}