using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using BBUnity.TestSupport;
using BBUnity.Entities.Controllers;
using BBUnity.Entities.Controllers.States;

namespace BBUnity.Entities.Tests {
    public class StateControllerTests {

        private class TrackingEntityState : EntityState {
            public int StartCallCount;
            public int EnterCallCount;
            public int ExitCallCount;

            public override void Start() { StartCallCount++; }
            public override void Enter() { EnterCallCount++; }
            public override void Exit() { ExitCallCount++; }
        }

        // Two distinct types (rather than reusing one) since the key is now derived from the
        // concrete type — two instances of the same type would collide on the same key.
        private class StubEntityStateA : TrackingEntityState { }
        private class StubEntityStateB : TrackingEntityState { }

        private class TransitioningEntityState : TrackingEntityState {
            public void GoTo<T>(bool forceTransition = false) where T : EntityState {
                SetState<T>(forceTransition);
            }
        }

        // Exposes the protected Start() so tests can drive the same registration pipeline Unity
        // would normally trigger via the component lifecycle (which EditMode tests don't run).
        private class TestStateController : StateController {
            public void InvokeStart() { Start(); }
        }

        [TearDown]
        public void TearDown() {
            TestUtilities.DestroyRootObjectsInScene();
        }

        // ----------------------------------------------------------------
        // EntityState.Key<T>() / KeyFor
        // ----------------------------------------------------------------

        [Test]
        public void Key_DerivesFromTypeFullName() {
            Assert.AreEqual(typeof(StubEntityStateA).FullName, EntityState.Key<StubEntityStateA>());
        }

        [Test]
        public void Key_DiffersForDifferentTypes() {
            Assert.AreNotEqual(EntityState.Key<StubEntityStateA>(), EntityState.Key<StubEntityStateB>());
        }

        // ----------------------------------------------------------------
        // StateController.AddState<T>()
        // ----------------------------------------------------------------

        [Test]
        public void AddStateGeneric_SetsStateControllerOnTheState() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            var state = new StubEntityStateA();

            controller.AddState(state);

            Assert.AreEqual(controller, state.StateController);
        }

        [Test]
        public void AddStateGeneric_RegistersUnderTypeDerivedKey() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            var state = new StubEntityStateA();

            controller.AddState(state);

            Assert.AreEqual(EntityState.Key<StubEntityStateA>(), state.ReferenceKey);
        }

        [Test]
        public void AddStateGeneric_TwoInstancesOfSameType_ThrowsOnDuplicateKey() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();

            controller.AddState(new StubEntityStateA());

            Assert.Throws<ArgumentException>(() => controller.AddState(new StubEntityStateA()));
        }

        // ----------------------------------------------------------------
        // EntityState.SetState<T>()
        // ----------------------------------------------------------------

        [Test]
        public void SetStateGeneric_TransitionsUsingTypeDerivedKey() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            var fromState = new TransitioningEntityState();
            var toState = new StubEntityStateA();
            controller.AddState(fromState, isDefault: true);
            controller.AddState(toState);
            controller.InvokeStart();

            fromState.GoTo<StubEntityStateA>();

            Assert.AreEqual(1, fromState.ExitCallCount);
            Assert.AreEqual(1, toState.EnterCallCount);
        }

        // ----------------------------------------------------------------
        // Start() — registration + default selection
        // ----------------------------------------------------------------

        [Test]
        public void Start_CallsStartOnEveryRegisteredState() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            var stateA = new StubEntityStateA();
            var stateB = new StubEntityStateB();
            controller.AddState(stateA, isDefault: true);
            controller.AddState(stateB);

            controller.InvokeStart();

            Assert.AreEqual(1, stateA.StartCallCount);
            Assert.AreEqual(1, stateB.StartCallCount);
        }

        [Test]
        public void Start_EntersOnlyTheDefaultState() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            var defaultState = new StubEntityStateA();
            var otherState = new StubEntityStateB();
            controller.AddState(otherState);
            controller.AddState(defaultState, isDefault: true);

            controller.InvokeStart();

            Assert.AreEqual(1, defaultState.EnterCallCount);
            Assert.AreEqual(0, otherState.EnterCallCount);
        }

        // ----------------------------------------------------------------
        // Editor-configured (SerializeReference) state list — the fields backing this are
        // private and only ever written by StateControllerInspector, so reflection is the only
        // way to set up this fixture from a runtime test.
        // ----------------------------------------------------------------

        [Test]
        public void Start_RegistersSerializedStatesList_EnteringOnlyTheMarkedDefault() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            var defaultState = new StubEntityStateA();
            var otherState = new StubEntityStateB();

            SetPrivateField(controller, "_states", new List<EntityState> { otherState, defaultState });
            SetPrivateField(controller, "_defaultState", defaultState);

            controller.InvokeStart();

            Assert.AreEqual(EntityState.Key<StubEntityStateA>(), defaultState.ReferenceKey);
            Assert.AreEqual(1, defaultState.StartCallCount);
            Assert.AreEqual(1, otherState.StartCallCount);
            Assert.AreEqual(1, defaultState.EnterCallCount);
            Assert.AreEqual(0, otherState.EnterCallCount);
        }

        [Test]
        public void Start_SerializedStatesList_SkipsNullEntriesWithoutThrowing() {
            var controller = TestUtilities.CreateGameObject<TestStateController>();
            SetPrivateField(controller, "_states", new List<EntityState> { null, new StubEntityStateA() });

            Assert.DoesNotThrow(() => controller.InvokeStart());
        }

        private static void SetPrivateField(object target, string fieldName, object value) {
            FieldInfo field = typeof(StateController).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }
    }
}
