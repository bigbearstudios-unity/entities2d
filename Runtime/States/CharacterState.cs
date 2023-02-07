namespace BBUnity.CharacterControllers.States {

    /// <summary>
    /// Representation of a single CharacterState. Follows the simple flow of:
    /// - Constuctor
    /// - OnEnter
    /// - OnExit
    /// - OnTick
    /// </summary>
    public abstract class CharacterState {

        /// <summary>
        /// The character controller
        /// </summary>
        protected CharacterController _character;
        protected CharacterController Character { get { return _character; } }

        // public CharacterState(BaseCharacter character) {
        //     _character = character;
        // }

        public virtual void OnTick() {}
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
    }
}