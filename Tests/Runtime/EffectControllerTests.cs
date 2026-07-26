using System.Text.RegularExpressions;

using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

using BBUnity.Pools;
using BBUnity.TestSupport;
using BBUnity.Entities.Controllers;

namespace BBUnity.Entities.Tests {

    // Issue 3 — InstantiateEffect used to index straight into a Dictionary and threw
    // an unhandled KeyNotFoundException for any name that wasn't registered.
    public class EffectControllerTests {

        [TearDown]
        public void TearDown() {
            TestUtilities.DestroyRootObjectsInScene();
        }

        [Test]
        public void InstantiateEffect_UnregisteredName_DoesNotThrow() {
            var controller = TestUtilities.CreateGameObject<EffectController>(name: "Effect Controller Under Test");

            LogAssert.Expect(LogType.Log, new Regex("creating one"));
            LogAssert.Expect(LogType.Error, new Regex("no registered effect named 'missing'"));
            Assert.DoesNotThrow(() => controller.InstantiateEffect("missing"));
        }

        [Test]
        public void InstantiateEffect_WithPosition_UnregisteredName_DoesNotThrow() {
            var controller = TestUtilities.CreateGameObject<EffectController>(name: "Effect Controller Under Test");

            LogAssert.Expect(LogType.Log, new Regex("creating one"));
            LogAssert.Expect(LogType.Error, new Regex("no registered effect named 'missing'"));
            Assert.DoesNotThrow(() => controller.InstantiateEffect("missing", Vector3.zero));
        }

        [Test]
        public void Awake_NoExistingPool_CreatesOneNamedEffectPool() {
            LogAssert.Expect(LogType.Log, new Regex("creating one"));
            TestUtilities.CreateGameObject<EffectController>(name: "Effect Controller Under Test");

            Assert.IsNotNull(ObjectPool.FindInScene("Effect Pool"));
        }

        [Test]
        public void Awake_ExistingPool_ReusesIt() {
            var existingPool = TestUtilities.CreateGameObject<ObjectPool>(name: "Effect Pool");

            TestUtilities.CreateGameObject<EffectController>(name: "Effect Controller Under Test");

            Assert.AreEqual(existingPool, ObjectPool.FindInScene("Effect Pool"));
        }
    }
}
