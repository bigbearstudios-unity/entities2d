using UnityEngine;
using BBUnity.Core2D.Physics;

namespace BBUnity.Entities.Characters.Components.Platforming {

    public interface ICollisionCharacterComponent : ICharacterComponent {
        void LeftGround();
        void LandedOnGround();
    }
}