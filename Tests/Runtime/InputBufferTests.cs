using NUnit.Framework;

using BBUnity.Entities.Controllers.Input.Actions;

namespace BBUnity.Entities.Tests {
    public class InputBufferTests {

        [Test]
        public void FreshBuffer_IsNotActive() {
            var buffer = new InputBuffer(0.1f);

            Assert.IsFalse(buffer.IsActive);
        }

        [Test]
        public void Trigger_MakesBufferActive() {
            var buffer = new InputBuffer(0.1f);

            buffer.Trigger();

            Assert.IsTrue(buffer.IsActive);
        }

        [Test]
        public void Tick_BelowThreshold_RemainsActive() {
            var buffer = new InputBuffer(0.1f);
            buffer.Trigger();

            buffer.Tick(0.05f);

            Assert.IsTrue(buffer.IsActive);
        }

        [Test]
        public void Tick_PastThreshold_BecomesInactive() {
            var buffer = new InputBuffer(0.1f);
            buffer.Trigger();

            buffer.Tick(0.2f);

            Assert.IsFalse(buffer.IsActive);
        }

        [Test]
        public void Tick_WithoutTrigger_StaysInactive() {
            var buffer = new InputBuffer(0.1f);

            buffer.Tick(0.01f);

            Assert.IsFalse(buffer.IsActive);
        }

        [Test]
        public void Trigger_AfterExpiry_ReactivatesBuffer() {
            var buffer = new InputBuffer(0.1f);
            buffer.Trigger();
            buffer.Tick(0.2f);
            Assert.IsFalse(buffer.IsActive);

            buffer.Trigger();

            Assert.IsTrue(buffer.IsActive);
        }

        [Test]
        public void Trigger_WhileActive_RestartsCountdown() {
            var buffer = new InputBuffer(0.1f);
            buffer.Trigger();
            buffer.Tick(0.08f);

            buffer.Trigger();
            buffer.Tick(0.08f);

            Assert.IsTrue(buffer.IsActive, "Re-triggering should restart the countdown rather than accumulate the previous remaining time");
        }
    }
}
