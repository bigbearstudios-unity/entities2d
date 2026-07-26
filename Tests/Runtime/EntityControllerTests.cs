using NUnit.Framework;

using BBUnity.TestSupport;
using BBUnity.Entities.Controllers.Base;

namespace BBUnity.Entities.Tests {
    public class EntityControllerTests {

        private class Target { }
        private class DerivedTarget : Target { }
        private class Unrelated { }
        private interface ITargetLike { }
        private class InterfaceImplementingTarget : Target, ITargetLike { }

        private class ReflectionTestDoubleBase : EntityController {
            // Private (not public) so this is only discoverable via FindReflectedFields'
            // explicit walk up the type hierarchy, not via the derived type's own GetFields
            // call (which already surfaces inherited public/protected members on its own —
            // mixing the two would double-count this field instead of testing the walk).
            private DerivedTarget _baseClassField = new DerivedTarget();

            public DerivedTarget BaseClassFieldValue { get { return _baseClassField; } }
        }

        private class ReflectionTestDouble : ReflectionTestDoubleBase {
            public Unrelated UnrelatedField = new Unrelated();
            public Target ExactTypeField = new Target();
            public DerivedTarget SubclassField = new DerivedTarget();
            public Target NullField = null;

            public Target[] FindTargets() {
                return FindReflectedFields<Target>();
            }
        }

        private class InterfaceReflectionTestDouble : EntityController {
            public InterfaceImplementingTarget ImplementingField = new InterfaceImplementingTarget();
            public Unrelated UnrelatedField = new Unrelated();

            public ITargetLike[] FindTargetLikes() {
                return FindReflectedFields<ITargetLike>();
            }
        }

        [Test]
        public void FindReflectedFields_FindsFieldsTypedAsASubclass() {
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                Target[] found = component.FindTargets();

                CollectionAssert.Contains(found, component.SubclassField);
            });
        }

        [Test]
        public void FindReflectedFields_IgnoresUnrelatedFieldTypes() {
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                Target[] found = component.FindTargets();

                CollectionAssert.DoesNotContain(found, component.UnrelatedField);
            });
        }

        [Test]
        public void FindReflectedFields_IncludesPrivateFieldsDeclaredOnBaseClasses() {
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                Target[] found = component.FindTargets();

                CollectionAssert.Contains(found, component.BaseClassFieldValue);
            });
        }

        [Test]
        public void FindReflectedFields_FindsExactTypeMatches() {
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                Target[] found = component.FindTargets();

                CollectionAssert.Contains(found, component.ExactTypeField);
            });
        }

        [Test]
        public void FindReflectedFields_FindsFieldsTypedAsAnImplementedInterface() {
            TestUtilities.CreateThenDestroyGameObject<InterfaceReflectionTestDouble>(component => {
                ITargetLike[] found = component.FindTargetLikes();

                CollectionAssert.Contains(found, component.ImplementingField);
                Assert.AreEqual(1, found.Length);
            });
        }

        [Test]
        public void FindReflectedFields_SkipsUnassignedFields() {
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                Target[] found = component.FindTargets();

                Assert.IsFalse(System.Array.Exists(found, value => value == null), "An unassigned (null) field should not appear as a null entry");
            });
        }

        [Test]
        public void FindReflectedFields_ReturnsEachMatchingFieldExactlyOnce() {
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                Target[] found = component.FindTargets();

                // ExactTypeField, SubclassField, BaseClassFieldValue — NullField is skipped.
                Assert.AreEqual(3, found.Length, "Fields declared on a base class must not be double-counted");
            });
        }

        [Test]
        public void FindReflectedFields_ReflectsCurrentValueAcrossRepeatedCalls() {
            // The set of matching FieldInfo is cached per-type, but the values read from those
            // fields must still be re-read fresh on every call, not cached alongside them.
            TestUtilities.CreateThenDestroyGameObject<ReflectionTestDouble>(component => {
                component.FindTargets();

                var replacement = new DerivedTarget();
                component.SubclassField = replacement;

                Target[] found = component.FindTargets();

                CollectionAssert.Contains(found, replacement);
            });
        }
    }
}
