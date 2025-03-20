using UnityEngine;
using UnityEditor;

using BBUnity.Entities.Controllers.Input;

[CustomEditor(typeof(PlayerUnityInputController))]
public class PlayerUnityInputControllerEditor : Editor {

    private PlayerUnityInputController Controller { 
        get { return (PlayerUnityInputController)target; }
    }
   
    public override void OnInspectorGUI () {
        DrawDefaultInspector();

        // TODO Add some warnings when stuff isn't added / setup

        if(GUILayout.Button("Add Action")) {
            Controller._Editor_AddButtonMapping();
        }

        // TODO
        // Add the ability to load in actions from the Unity Input
   }
}