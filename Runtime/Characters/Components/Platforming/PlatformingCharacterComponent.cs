using UnityEngine;

using BBUnity.Entities;
using BBUnity.Core2D.Movement;
using BBUnity.Entities.Characters.Platforming;

namespace BBUnity.Entities.Characters.Components.Platforming {
    public class PlatformingCharacterComponent : CharacterComponent {
        
        /// <summary>
        /// Internal method to cast the character instance to a
        /// PlatformingCharacter. This casting is always avalible.
        /// </summary>
        /// <value></value>
        internal PlatformingCharacter PlatformingCharacter {
            get { return (PlatformingCharacter)_character; }
        }

        /*
         * Component Accessors
         */
        protected CollisionCharacterComponent CollisionComponent {
            get { return PlatformingCharacter.CollisionComponent; }
        }

        protected InputCharacterComponent InputComponent {
            get { return PlatformingCharacter.InputComponent; }
        }

        protected MovementCharacterComponent MovementComponent {
            get { return PlatformingCharacter.MovementComponent; }
        }
    }
}