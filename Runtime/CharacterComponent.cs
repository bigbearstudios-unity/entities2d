using UnityEngine;

namespace BBUnity.GameEntities {

    /// <summary>
    /// The base CharacterComponent. Character components are POD objects
    /// which have a single purpose. E.g. Controlling Movement, Controlling Collision
    /// or controlling input.
    /// 
    /// They generally consist of the ability to interact with the controller which will
    /// change what it does on its update loop, then the ability to read information
    /// from the controller.
    /// 
    /// </summary>
    public class CharacterComponent {

        /// <summary>
        /// The MonoBehaviour which owns the controller.
        /// </summary>
        private Character _character;
        protected Character Character { get { return _character; } }

        public CharacterComponent() { }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnDrawGizmos() { }
    }
}