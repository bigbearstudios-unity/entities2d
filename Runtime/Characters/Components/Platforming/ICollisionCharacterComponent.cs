namespace BBUnity.Entities.Characters.Components.Platforming {
    public interface ICollisionCharacterComponent : ICharacterComponent {
        void LeftGround();
        void LandedOnGround();
    }
}