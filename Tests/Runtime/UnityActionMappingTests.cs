using NUnit.Framework;

using BBUnity.Entities.Controllers.Input.Mappings;

namespace BBUnity.Entities.Tests {
    public class UnityActionMappingTests {
    
        [Test]
        public void MovementMapping_ReportsMovementType() {
            var mapping = new UnityMovementActionMapping();

            Assert.IsTrue(mapping.IsMovementType);
            Assert.IsFalse(mapping.IsButtonType);
            Assert.AreEqual(MappingTypes.Movement, mapping.Type);
        }

        [Test]
        public void ButtonMapping_ReportsButtonType() {
            var mapping = new UnityButtonActionMapping();

            Assert.IsTrue(mapping.IsButtonType);
            Assert.IsFalse(mapping.IsMovementType);
            Assert.AreEqual(MappingTypes.Button, mapping.Type);
        }

        [Test]
        public void AxisMapping_ReportsNeitherMovementNorButtonType() {
            var mapping = new UnityAxisActionMapping();

            Assert.IsFalse(mapping.IsMovementType);
            Assert.IsFalse(mapping.IsButtonType);
            Assert.AreEqual(MappingTypes.Axis, mapping.Type);
        }

        [Test]
        public void MovementMapping_DefaultsActionNameToMovement() {
            var mapping = new UnityMovementActionMapping();

            Assert.AreEqual("Movement", mapping.Action);
        }

        [Test]
        public void ButtonMapping_ZeroBufferThreshold_BuildsUnbufferedAction() {
            var mapping = new UnityButtonActionMapping();

            Assert.AreEqual(0.0f, mapping.BufferThreshold);
        }
    }
}
