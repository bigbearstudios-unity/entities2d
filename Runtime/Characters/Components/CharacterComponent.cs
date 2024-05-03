using UnityEngine;

namespace BBUnity.Entities.Characters.Components {

    /// <summary>
    /// 
    /// </summary>
    public class CharacterComponent {

        /*
         * Character, getters / setter
         * Due to the instances of these component being instanciated via the UI
         * the character instance needs to be set via the SetCharacter method before
         * any interactions with the Components can begin
         */
        protected Character _character;
        internal void SetCharacter(Character character) {
            _character = character;
        }

        /// <summary>
        /// The delegate which will recieve callbacks from
        /// the component itself
        /// </summary>
        protected ICharacterComponent _componentDelegate;

        /*
         * Unity Pass-throughs
         * These are Unity specific methods which will be called directly on the
         * underlying _character instance
         */

        protected Transform transform { get { return _character.transform; } }

        /*
         * Constructors
         * All Character Components should always have an empty constructor when it comes
         * to params. This is because components will always have [System.Serializable]
         * and thus be serialized by Unity using the EditorUI
         */

        public CharacterComponent() {}

        public virtual void Awake() {}
        public virtual void Start() {}
        public virtual void Update() {}
        public virtual void OnDrawGizmos() {}
    }
}