using System.Collections;

using NUnit.Framework;
using UnityEngine.TestTools;

using BBUnity.Movement;
using BBUnity.TestSupport;

namespace BBUnity.Entities.Tests {

    // PlayMode tests: CoyoteTime measures real elapsed Time.time, so its threshold
    // behaviour needs actual frames to pass rather than a mocked clock.
    public class CoyoteTimeTests {

        [Test]
        public void FreshInstance_IsNotAvailable() {
            var coyoteTime = new CoyoteTime();

            Assert.IsFalse(coyoteTime.IsAvailable);
        }

        [Test]
        public void StartTimer_WithoutSetAvailable_IsNotAvailable() {
            var coyoteTime = new CoyoteTime();

            coyoteTime.StartTimer();

            Assert.IsFalse(coyoteTime.IsAvailable, "StartTimer alone should not grant availability — SetAvailable(true) is also required");
        }

        [Test]
        public void SetAvailable_WithoutStartTimer_IsNotAvailable() {
            // Issue 16 — before the fix, _startedAt defaulted to float.MinValue so
            // (Time.time - _startedAt) was always huge and this "worked by accident".
            // With an explicit started flag it must be false regardless of arithmetic.
            var coyoteTime = new CoyoteTime();

            coyoteTime.SetAvailable(true);

            Assert.IsFalse(coyoteTime.IsAvailable);
        }

        [Test]
        public void StartTimerAndSetAvailable_IsImmediatelyAvailable() {
            var coyoteTime = new CoyoteTime();

            coyoteTime.StartTimer();
            coyoteTime.SetAvailable(true);

            Assert.IsTrue(coyoteTime.IsAvailable);
        }

        [Test]
        public void Reset_ClearsAvailability() {
            var coyoteTime = new CoyoteTime();
            coyoteTime.StartTimer();
            coyoteTime.SetAvailable(true);

            coyoteTime.Reset();

            Assert.IsFalse(coyoteTime.IsAvailable);
        }

        [UnityTest]
        public IEnumerator IsAvailable_BecomesFalse_AfterThresholdElapses() {
            var coyoteTime = new CoyoteTime(); // default threshold is 0.1s
            coyoteTime.StartTimer();
            coyoteTime.SetAvailable(true);

            Assert.IsTrue(coyoteTime.IsAvailable, "should still be available immediately after starting");

            yield return TestUtilities.WaitForSeconds(0.2f);

            Assert.IsFalse(coyoteTime.IsAvailable, "should no longer be available once the threshold has elapsed");
        }
    }
}
