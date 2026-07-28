using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

using BBUnity.Entities.Controllers.States;

namespace BBUnity.Entities.Controllers.EditorTools {

    /// <summary>
    /// Searchable "pick a state type" dropdown, nested by namespace (mirroring Unity's own
    /// Add Component menu) so same-named states in different namespaces — e.g. a Player and an
    /// Enemy GroundedIdleState — stay unambiguous.
    /// </summary>
    internal class StateTypeDropdown : AdvancedDropdown {
        private readonly List<Type> _types;
        private readonly Action<Type> _onTypeSelected;

        public StateTypeDropdown(AdvancedDropdownState state, List<Type> types, Action<Type> onTypeSelected) : base(state) {
            _types = types;
            _onTypeSelected = onTypeSelected;
            minimumSize = new Vector2(350, 350);
        }

        protected override AdvancedDropdownItem BuildRoot() {
            var root = new AdvancedDropdownItem("State Type");

            for (int i = 0; i < _types.Count; i++) {
                Type type = _types[i];
                AdvancedDropdownItem parent = root;

                string[] namespaceSegments = (type.Namespace ?? string.Empty).Split('.');
                foreach (string segment in namespaceSegments) {
                    if (segment.Length == 0) { continue; }
                    parent = FindOrAddChild(parent, segment);
                }

                parent.AddChild(new AdvancedDropdownItem(type.Name) { id = i });
            }

            return root;
        }

        private static AdvancedDropdownItem FindOrAddChild(AdvancedDropdownItem parent, string name) {
            foreach (AdvancedDropdownItem child in parent.children) {
                if (child.name == name) { return child; }
            }

            var newChild = new AdvancedDropdownItem(name);
            parent.AddChild(newChild);
            return newChild;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            if (item.id < 0 || item.id >= _types.Count) { return; }
            _onTypeSelected(_types[item.id]);
        }
    }

    [CustomEditor(typeof(StateController), editorForChildClasses: true)]
    public class StateControllerInspector : UnityEditor.Editor {

        private static List<Type> _cachedStateTypes;

        private ReorderableList _list;
        private SerializedProperty _statesProperty;
        private SerializedProperty _defaultStateProperty;

        private void OnEnable() {
            _statesProperty = serializedObject.FindProperty("_states");
            _defaultStateProperty = serializedObject.FindProperty("_defaultState");

            _list = new ReorderableList(serializedObject, _statesProperty, true, true, false, true);
            _list.drawHeaderCallback += DrawHeader;
            _list.drawElementCallback += DrawElement;
            _list.elementHeightCallback += GetElementHeight;
            _list.onRemoveCallback += RemoveElement;
        }

        private void OnDisable() {
            if (_list == null) { return; }

            _list.drawHeaderCallback -= DrawHeader;
            _list.drawElementCallback -= DrawElement;
            _list.elementHeightCallback -= GetElementHeight;
            _list.onRemoveCallback -= RemoveElement;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "_states", "_defaultState", "m_Script");

            EditorGUILayout.Space();
            _list.DoLayoutList();
            DrawAddStateButton();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect) {
            GUI.Label(rect, "States");
        }

        private float GetElementHeight(int index) {
            return EditorGUIUtility.singleLineHeight * 2 + 6;
        }

        private void DrawElement(Rect rect, int index, bool active, bool focused) {
            SerializedProperty element = _statesProperty.GetArrayElementAtIndex(index);
            EntityState instance = element.managedReferenceValue as EntityState;

            var typeRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            string label = instance != null ? instance.GetType().FullName : "(missing state)";
            EditorGUI.LabelField(typeRect, label, EditorStyles.boldLabel);

            if (instance == null) { return; }

            var defaultRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 4, 150, EditorGUIUtility.singleLineHeight);
            bool isDefault = ReferenceEquals(instance, _defaultStateProperty.managedReferenceValue);
            bool newIsDefault = EditorGUI.ToggleLeft(defaultRect, "Default", isDefault);

            if (newIsDefault != isDefault) {
                _defaultStateProperty.managedReferenceValue = newIsDefault ? instance : null;
            }
        }

        private void RemoveElement(ReorderableList list) {
            SerializedProperty element = _statesProperty.GetArrayElementAtIndex(list.index);
            object removedInstance = element.managedReferenceValue;

            if (ReferenceEquals(_defaultStateProperty.managedReferenceValue, removedInstance)) {
                _defaultStateProperty.managedReferenceValue = null;
            }

            _statesProperty.DeleteArrayElementAtIndex(list.index);
        }

        private void DrawAddStateButton() {
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent("Add State"), GUI.skin.button);
            if (!GUI.Button(buttonRect, "Add State")) { return; }

            var dropdown = new StateTypeDropdown(new AdvancedDropdownState(), GetAvailableStateTypes(), AddState);
            dropdown.Show(buttonRect);
        }

        private void AddState(Type type) {
            var instance = Activator.CreateInstance(type) as EntityState;

            serializedObject.Update();
            _statesProperty.arraySize++;
            SerializedProperty newElement = _statesProperty.GetArrayElementAtIndex(_statesProperty.arraySize - 1);
            newElement.managedReferenceValue = instance;
            serializedObject.ApplyModifiedProperties();
        }

        private static List<Type> GetAvailableStateTypes() {
            if (_cachedStateTypes == null) {
                _cachedStateTypes = TypeCache.GetTypesDerivedFrom<EntityState>()
                    .Where(t => !t.IsAbstract && !t.IsNested && !IsFromTestAssembly(t) && HasParameterlessConstructor(t))
                    .OrderBy(t => t.FullName)
                    .ToList();
            }

            return _cachedStateTypes;
        }

        private static bool HasParameterlessConstructor(Type type) {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        // Excludes test-double state classes (e.g. StateControllerTests' private nested stubs)
        // from the picker. Nested types are already caught by !t.IsNested above — this also
        // catches any future test double declared as a top-level type in a *.Tests assembly.
        private static bool IsFromTestAssembly(Type type) {
            return type.Assembly.GetName().Name.EndsWith(".Tests", StringComparison.Ordinal);
        }
    }
}
