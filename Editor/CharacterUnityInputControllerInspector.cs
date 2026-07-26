using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using BBUnity.Entities.Controllers.Input;

[CustomEditor(typeof(PlayerUnityInputController))]
public class PlayerUnityInputControllerEditor : Editor {

    private PlayerUnityInputController Controller {
        get { return (PlayerUnityInputController)target; }
    }

    public override void OnInspectorGUI () {
        serializedObject.Update();

        SerializedProperty playerInputProperty = serializedObject.FindProperty("_playerInput");
        SerializedProperty mappingsProperty = serializedObject.FindProperty("_actionMappings");

        EditorGUILayout.PropertyField(playerInputProperty);
        EditorGUILayout.PropertyField(mappingsProperty);

        serializedObject.ApplyModifiedProperties();

        DrawSetupWarnings(playerInputProperty, mappingsProperty);

        if (GUILayout.Button("Add Action")) {
            Undo.RecordObject(Controller, "Add Action Mapping");
            Controller._Editor_AddButtonMapping();
        }

        DrawPopulateFromInputActions(playerInputProperty, mappingsProperty);
    }

    /// <summary>
    /// Surfaces the setup problems this controller would otherwise only reveal by throwing
    /// at runtime: a missing Player Input, a Player Input with no actions asset, no mappings
    /// configured at all, or a mapping whose action name doesn't exist in the assigned asset.
    /// </summary>
    private void DrawSetupWarnings(SerializedProperty playerInputProperty, SerializedProperty mappingsProperty) {
        var playerInput = playerInputProperty.objectReferenceValue as UnityEngine.InputSystem.PlayerInput;

        if (playerInput == null) {
            EditorGUILayout.HelpBox("No Player Input component assigned — this controller will throw on Awake().", MessageType.Error);
            return;
        }

        if (playerInput.actions == null) {
            EditorGUILayout.HelpBox("The assigned Player Input has no Input Actions asset set.", MessageType.Error);
            return;
        }

        if (mappingsProperty.arraySize == 0) {
            EditorGUILayout.HelpBox("No action mappings configured.", MessageType.Warning);
            return;
        }

        List<string> problemMappings = new List<string>();
        for (int i = 0; i < mappingsProperty.arraySize; i++) {
            SerializedProperty actionNameProperty = mappingsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_action");
            string actionName = actionNameProperty != null ? actionNameProperty.stringValue : null;

            if (string.IsNullOrEmpty(actionName)) {
                problemMappings.Add("(unnamed mapping)");
            } else if (playerInput.actions.FindAction(actionName) == null) {
                problemMappings.Add(actionName);
            }
        }

        if (problemMappings.Count > 0) {
            EditorGUILayout.HelpBox(
                "These mappings don't match any action in the assigned Input Actions asset: " + string.Join(", ", problemMappings),
                MessageType.Warning
            );
        }
    }

    /// <summary>
    /// Lists actions present in the assigned Player Input's actions asset that don't have a
    /// mapping yet, with a one-click way to add one — you still choose whether it should be a
    /// button or an axis mapping.
    /// </summary>
    private void DrawPopulateFromInputActions(SerializedProperty playerInputProperty, SerializedProperty mappingsProperty) {
        var playerInput = playerInputProperty.objectReferenceValue as UnityEngine.InputSystem.PlayerInput;
        if (playerInput == null || playerInput.actions == null) { return; }

        HashSet<string> mappedActionNames = new HashSet<string>();
        for (int i = 0; i < mappingsProperty.arraySize; i++) {
            SerializedProperty actionNameProperty = mappingsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_action");
            if (actionNameProperty != null && !string.IsNullOrEmpty(actionNameProperty.stringValue)) {
                mappedActionNames.Add(actionNameProperty.stringValue);
            }
        }

        List<string> unmappedActionNames = playerInput.actions
            .Select(action => action.name)
            .Distinct()
            .Where(name => !mappedActionNames.Contains(name))
            .ToList();

        if (unmappedActionNames.Count == 0) { return; }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions not yet mapped", EditorStyles.boldLabel);

        foreach (string actionName in unmappedActionNames) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(actionName);

            if (GUILayout.Button("+ Button", GUILayout.Width(70))) {
                AddMapping(actionName, isButton: true);
            }

            if (GUILayout.Button("+ Axis", GUILayout.Width(70))) {
                AddMapping(actionName, isButton: false);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void AddMapping(string actionName, bool isButton) {
        Undo.RecordObject(Controller, "Add Action Mapping");

        if (isButton) {
            Controller._Editor_AddButtonMapping();
        } else {
            Controller._Editor_AddAxisMapping();
        }

        serializedObject.Update();

        SerializedProperty mappingsProperty = serializedObject.FindProperty("_actionMappings");
        SerializedProperty newMapping = mappingsProperty.GetArrayElementAtIndex(mappingsProperty.arraySize - 1);
        SerializedProperty actionNameProperty = newMapping.FindPropertyRelative("_action");
        if (actionNameProperty != null) {
            actionNameProperty.stringValue = actionName;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
