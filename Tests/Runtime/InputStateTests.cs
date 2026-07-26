using NUnit.Framework;

using BBUnity.Entities.Controllers.Platforming.Internal;

namespace BBUnity.Entities.Tests {
    public class InputStateTests {

        [Test]
        public void FreshState_HasNoHorizontalMovement() {
            var state = new InputState();

            Assert.IsFalse(state.HasHorizontalMovement);
            Assert.AreEqual(0.0f, state.HorizontalMovement);
        }

        [Test]
        public void SetHorizontalMovement_WithoutSnap_PreservesValue() {
            var state = new InputState();

            state.SetHorizontalMovement(0.35f, snapMovement: false);

            Assert.AreEqual(0.35f, state.HorizontalMovement);
            Assert.IsTrue(state.HasHorizontalMovement);
        }

        [Test]
        public void SetHorizontalMovement_WithSnap_SnapsToSign() {
            var state = new InputState();

            state.SetHorizontalMovement(0.35f, snapMovement: true);

            Assert.AreEqual(1.0f, state.HorizontalMovement);
        }

        [Test]
        public void SetHorizontalMovement_WithSnap_NegativeSnapsToNegativeOne() {
            var state = new InputState();

            state.SetHorizontalMovement(-0.35f, snapMovement: true);

            Assert.AreEqual(-1.0f, state.HorizontalMovement);
        }

        [Test]
        public void SetHorizontalMovement_ZeroWithSnap_StaysZero() {
            var state = new InputState();

            state.SetHorizontalMovement(0.0f, snapMovement: true);

            Assert.AreEqual(0.0f, state.HorizontalMovement);
            Assert.IsFalse(state.HasHorizontalMovement);
        }

        [Test]
        public void SetJump_ExposesJumpAndJumpPressed() {
            var state = new InputState();

            state.SetJump(jump: true, jumpPressed: false);

            Assert.IsTrue(state.Jump);
            Assert.IsFalse(state.JumpPressed);
        }
    }
}
