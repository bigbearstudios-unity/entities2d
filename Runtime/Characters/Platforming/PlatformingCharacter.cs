using System;
using UnityEngine;

using BBUnity.Entities.Characters.Components;
using BBUnity.Entities.Characters.Components.Platforming;

namespace BBUnity.Entities.Characters.Platforming {
    public class PlatformingCharacter : Character {

        internal class NullComponentException : Exception {
            public NullComponentException() { }
            public NullComponentException(string message) : base(message) {}
            public NullComponentException(string message, Exception inner) : base(message, inner) {}
        } 

        /*
         * Character Component References
         * These are all of the base references and will be overridden
         * by specialised subclasses. 
         */

        public virtual MovementCharacterComponent MovementComponent { get { return null; } }
        public virtual CollisionCharacterComponent CollisionComponent { get { return null; } }
        public virtual InputCharacterComponent InputComponent { get { return null; } }

        private void RegisterInternalComponents() {
            if(MovementComponent == null) throw new NullComponentException("MovementComponent required. TODO");
            if(CollisionComponent == null) throw new NullComponentException("CollisionComponent required. TODO");
            if(InputComponent == null) throw new NullComponentException("InputComponent required. TODO");

            RegisterComponents(
                new CharacterComponent[]{
                    InputComponent,
                    CollisionComponent,
                    MovementComponent
                }
            );
        }

        protected virtual void Awake() {
            RegisterInternalComponents();
            SetCharacterOnAllComponents();
            AwakeAllComponents();

            OnAwake();
        }

        protected virtual void Start() {
            StartAllComponents();
            
            OnStart();
        }

        /// <summary>
        /// Overriden Update logic where we update
        /// the majority of the components before 
        /// post updating the movement controller
        /// </summary>
        protected virtual void Update() {
            InputComponent.Update();
            CollisionComponent.Update();

            OnUpdate();

            MovementComponent.Update();
        }

        protected virtual void OnDrawGizmos() {
            RegisterInternalComponents();
            SetCharacterOnAllComponents();
            DrawGizmosOnAllComponents();
        }
    }
}