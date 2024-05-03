using UnityEngine;
using BBUnity.Entities.Characters.Components;

namespace BBUnity.Entities.Characters {

    /// <summary>
    /// The base class for all characters
    /// </summary>
    public abstract class Character : Entity, ICharacter {
        
        /// <summary>
        /// All of the components which are registered to the Character
        /// </summary>
        protected CharacterComponent[] _components;
        protected void RegisterComponents(CharacterComponent[] components) {
            _components = components;
        }

        protected void SetCharacterOnAllComponents() {
            foreach(CharacterComponent component in _components) {
                component.SetCharacter(this);
            }
        }

        protected void AwakeAllComponents() {
            foreach(CharacterComponent component in _components) {
                component.Awake();
            }
        }

        protected void StartAllComponents() {
            foreach(CharacterComponent component in _components) {
                component.Start();
            }
        }

        protected void UpdateAllComponents() {
            foreach(CharacterComponent component in _components) {
                component.Update();
            }
        }

        protected void DrawGizmosOnAllComponents() {
            foreach(CharacterComponent component in _components) {
                component.OnDrawGizmos();
            }
        }
    }
}