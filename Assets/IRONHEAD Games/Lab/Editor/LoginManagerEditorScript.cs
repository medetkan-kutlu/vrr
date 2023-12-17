using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(LoginManager)), CanEditMultipleObjects]
public class LoginManagerEditorScript : Editor
{
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("This script is responsible for", MessageType.Info);

        LoginManager loginManager  = (LoginManager)target;
        if(GUILayout.Button("Connect to Photon")){
            loginManager.ConnectToPhoton();
        }

    }
}
